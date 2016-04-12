(function () {
    var nv = window.nv || {};
    nv.version = '1.1.15b';
    nv.dev = false; //set false when in production

    nv.tooltip = nv.tooltip || {}; // For the tooltip system
    nv.utils = nv.utils || {}; // Utility subsystem
    nv.models = nv.models || {}; //stores all the possible models/components
    nv.charts = {}; //stores all the ready to use charts
    nv.graphs = []; //stores all the graphs currently on the page
    nv.logs = {}; //stores some statistics and potential error messages

    nv.dispatch = d3.dispatch('render_start', 'render_end');

    // *************************************************************************
    //  Development render timers - disabled if dev = false

    if (nv.dev) {
        nv.dispatch.on('render_start', function (e) {
            nv.logs.startTime = +new Date();
        });

        nv.dispatch.on('render_end', function (e) {
            nv.logs.endTime = +new Date();
            nv.logs.totalTime = nv.logs.endTime - nv.logs.startTime;
            nv.log('total', nv.logs.totalTime); // used for development, to keep track of graph generation times
        });
    }

    // ********************************************
    //  Public Core NV functions

    // Logs all arguments, and returns the last so you can test things in place
    // Note: in IE8 console.log is an object not a function, and if modernizr is used
    // then calling Function.prototype.bind with with anything other than a function
    // causes a TypeError to be thrown.
    nv.log = function () {
        if (nv.dev && console.log && console.log.apply)
            console.log.apply(console, arguments)
        else if (nv.dev && typeof console.log == "function" && Function.prototype.bind) {
            var log = Function.prototype.bind.call(console.log, console);
            log.apply(console, arguments);
        }
        return arguments[arguments.length - 1];
    };

    nv.render = function render(step) {
        step = step || 1; // number of graphs to generate in each timeout loop

        nv.render.active = true;
        nv.dispatch.render_start();

        setTimeout(function () {
            var chart, graph;

            for (var i = 0; i < step && (graph = nv.render.queue[i]) ; i++) {
                chart = graph.generate();
                if (typeof graph.callback == typeof (Function)) graph.callback(chart);
                nv.graphs.push(chart);
            }

            nv.render.queue.splice(0, i);

            if (nv.render.queue.length) setTimeout(arguments.callee, 0);
            else {
                nv.dispatch.render_end();
                nv.render.active = false;
            }
        }, 0);
    };

    nv.render.active = false;
    nv.render.queue = [];

    nv.addGraph = function (obj) {
        if (typeof arguments[0] === typeof (Function))
            obj = { generate: arguments[0], callback: arguments[1] };

        nv.render.queue.push(obj);

        if (!nv.render.active) nv.render();
    };

    nv.identity = function (d) { return d; };

    nv.strip = function (s) { return s.replace(/(\s|&)/g, ''); };

    function daysInMonth(month, year) {
        return (new Date(year, month + 1, 0)).getDate();
    }

    function d3_time_range(floor, step, number) {
        return function (t0, t1, dt) {
            var time = floor(t0), times = [];
            if (time < t0) step(time);
            if (dt > 1) {
                while (time < t1) {
                    var date = new Date(+time);
                    if ((number(date) % dt === 0)) times.push(date);
                    step(time);
                }
            } else {
                while (time < t1) { times.push(new Date(+time)); step(time); }
            }
            return times;
        };
    }

    d3.time.monthEnd = function (date) {
        return new Date(date.getFullYear(), date.getMonth(), 0);
    };

    d3.time.monthEnds = d3_time_range(d3.time.monthEnd, function (date) {
        date.setUTCDate(date.getUTCDate() + 1);
        date.setDate(daysInMonth(date.getMonth() + 1, date.getFullYear()));
    }, function (date) {
        return date.getMonth();
    });


    nv.utils.windowSize = function () {
        // Sane defaults
        var size = { width: 640, height: 480 };

        // Earlier IE uses Doc.body
        if (document.body && document.body.offsetWidth) {
            size.width = document.body.offsetWidth;
            size.height = document.body.offsetHeight;
        }

        // IE can use depending on mode it is in
        if (document.compatMode == 'CSS1Compat' &&
            document.documentElement &&
            document.documentElement.offsetWidth) {
            size.width = document.documentElement.offsetWidth;
            size.height = document.documentElement.offsetHeight;
        }

        // Most recent browsers use
        if (window.innerWidth && window.innerHeight) {
            size.width = window.innerWidth;
            size.height = window.innerHeight;
        }
        return (size);
    };

    // Easy way to bind multiple functions to window.onresize
    // TODO: give a way to remove a function after its bound, other than removing all of them
    nv.utils.windowResize = function (fun) {
        if (fun === undefined) return;
        var oldresize = window.onresize;

        window.onresize = function (e) {
            if (typeof oldresize == 'function') oldresize(e);
            fun(e);
        }
    }

    // Backwards compatible way to implement more d3-like coloring of graphs.
    // If passed an array, wrap it in a function which implements the old default
    // behavior
    nv.utils.getColor = function (color) {
        if (!arguments.length) return nv.utils.defaultColor(); //if you pass in nothing, get default colors back

        if (Object.prototype.toString.call(color) === '[object Array]')
            return function (d, i) { return d.color || color[i % color.length]; };
        else
            return color;
        //can't really help it if someone passes rubbish as color
    }

    // Default color chooser uses the index of an object as before.
    nv.utils.defaultColor = function () {
        var colors = d3.scale.category20().range();
        return function (d, i) { return d.color || colors[i % colors.length] };
    }

    // Returns a color function that takes the result of 'getKey' for each series and
    // looks for a corresponding color from the dictionary,
    nv.utils.customTheme = function (dictionary, getKey, defaultColors) {
        getKey = getKey || function (series) { return series.key }; // use default series.key if getKey is undefined
        defaultColors = defaultColors || d3.scale.category20().range(); //default color function

        var defIndex = defaultColors.length; //current default color (going in reverse)

        return function (series, index) {
            var key = getKey(series);

            if (!defIndex) defIndex = defaultColors.length; //used all the default colors, start over

            if (typeof dictionary[key] !== "undefined")
                return (typeof dictionary[key] === "function") ? dictionary[key]() : dictionary[key];
            else
                return defaultColors[--defIndex]; // no match in dictionary, use default color
        }
    }

    // From the PJAX example on d3js.org, while this is not really directly needed
    // it's a very cool method for doing pjax, I may expand upon it a little bit,
    // open to suggestions on anything that may be useful
    nv.utils.pjax = function (links, content) {
        d3.selectAll(links).on("click", function () {
            history.pushState(this.href, this.textContent, this.href);
            load(this.href);
            d3.event.preventDefault();
        });

        function load(href) {
            d3.html(href, function (fragment) {
                var target = d3.select(content).node();
                target.parentNode.replaceChild(d3.select(fragment).select(content).node(), target);
                nv.utils.pjax(links, content);
            });
        }

        d3.select(window).on("popstate", function () {
            if (d3.event.state) load(d3.event.state);
        });
    }

    /* For situations where we want to approximate the width in pixels for an SVG:text element.
    Most common instance is when the element is in a display:none; container.
    Forumla is : text.length * font-size * constant_factor
    */
    nv.utils.calcApproxTextWidth = function (svgTextElem) {
        if (typeof svgTextElem.style === 'function'
            && typeof svgTextElem.text === 'function') {
            var fontSize = parseInt(svgTextElem.style("font-size").replace("px", ""));
            var textLength = svgTextElem.text().length;

            return textLength * fontSize * 0.5;
        }
        return 0;
    };

    /* Numbers that are undefined, null or NaN, convert them to zeros.
    */
    nv.utils.NaNtoZero = function (n) {
        if (typeof n !== 'number'
            || isNaN(n)
            || n === null
            || n === Infinity) return 0;

        return n;
    };

    /*
    Snippet of code you can insert into each nv.models.* to give you the ability to
    do things like:
    chart.options({
      showXAxis: true,
      tooltips: true
    });
    
    To enable in the chart:
    chart.options = nv.utils.optionsFunc.bind(chart);
    */
    nv.utils.optionsFunc = function (args) {
        if (args) {
            d3.map(args).forEach((function (key, value) {
                if (typeof this[key] === "function") {
                    this[key](value);
                }
            }).bind(this));
        }
        return this;
    };

    nv.models.legend = function () {
        "use strict";
        //============================================================
        // Public Variables with Default Settings
        //------------------------------------------------------------

        var margin = { top: 5, right: 0, bottom: 5, left: 0 }
          , width = 400
          , height = 20
          , getKey = function (d) { return d.key }
          , color = nv.utils.defaultColor()
          , align = true
          , rightAlign = false
          , updateState = true   //If true, legend will update data.disabled and trigger a 'stateChange' dispatch.
          , radioButtonMode = false   //If true, clicking legend items will cause it to behave like a radio button. (only one can be selected at a time)
          , dispatch = d3.dispatch('legendClick', 'legendDblclick', 'legendMouseover', 'legendMouseout', 'stateChange')
        ;

        //============================================================


        function chart(selection) {
            selection.each(function (data) {
                var availableWidth = width - margin.left - margin.right,
                    container = d3.select(this);

                //------------------------------------------------------------
                // Setup containers and skeleton of chart

                var wrap = container.selectAll('g.nv-legend').data([data]);
                var gEnter = wrap.enter().append('g').attr('class', 'nvd3 nv-legend').append('g');
                var g = wrap.select('g');

                wrap.attr('transform', 'translate(' + margin.left + ',' + margin.top + ')');

                //------------------------------------------------------------


                var series = g.selectAll('.nv-series')
                    .data(function (d) { return d });
                var seriesEnter = series.enter().append('g').attr('class', 'nv-series')
                    .on('mouseover', function (d, i) {
                        dispatch.legendMouseover(d, i);  //TODO: Make consistent with other event objects
                    })
                    .on('mouseout', function (d, i) {
                        dispatch.legendMouseout(d, i);
                    })
                    .on('click', function (d, i) {
                        dispatch.legendClick(d, i);
                        if (updateState) {
                            if (radioButtonMode) {
                                //Radio button mode: set every series to disabled,
                                //  and enable the clicked series.
                                data.forEach(function (series) { series.disabled = true });
                                d.disabled = false;
                            }
                            else {
                                d.disabled = !d.disabled;
                                if (data.every(function (series) { return series.disabled })) {
                                    //the default behavior of NVD3 legends is, if every single series
                                    // is disabled, turn all series' back on.
                                    data.forEach(function (series) { series.disabled = false });
                                }
                            }
                            dispatch.stateChange({
                                disabled: data.map(function (d) { return !!d.disabled })
                            });
                        }
                    })
                    .on('dblclick', function (d, i) {
                        dispatch.legendDblclick(d, i);
                        if (updateState) {
                            //the default behavior of NVD3 legends, when double clicking one,
                            // is to set all other series' to false, and make the double clicked series enabled.
                            data.forEach(function (series) {
                                series.disabled = true;
                            });
                            d.disabled = false;
                            dispatch.stateChange({
                                disabled: data.map(function (d) { return !!d.disabled })
                            });
                        }
                    });
                seriesEnter.append('circle')
                    .style('stroke-width', 2)
                    .attr('class', 'nv-legend-symbol')
                    .attr('r', 5);
                seriesEnter.append('text')
                    .attr('text-anchor', 'start')
                    .attr('class', 'nv-legend-text')
                    .attr('dy', '.32em')
                    .attr('dx', '8');
                series.classed('disabled', function (d) { return d.disabled });
                series.exit().remove();
                series.select('circle')
                    .style('fill', function (d, i) { return d.color || color(d, i) })
                    .style('stroke', function (d, i) { return d.color || color(d, i) });
                series.select('text')
                    .text(function (d, i) {
                        return d.DataKey + ' - ' + d.DataValue;
                    });


                //TODO: implement fixed-width and max-width options (max-width is especially useful with the align option)

                // NEW ALIGNING CODE, TODO: clean up
                if (align) {

                    var seriesWidths = [];
                    series.each(function (d, i) {
                        var legendText = d3.select(this).select('text');
                        var nodeTextLength;
                        try {
                            nodeTextLength = legendText.node().getComputedTextLength();
                            // If the legendText is display:none'd (nodeTextLength == 0), simulate an error so we approximate, instead
                            if (nodeTextLength <= 0) throw Error();
                        }
                        catch (e) {
                            nodeTextLength = nv.utils.calcApproxTextWidth(legendText);
                        }

                        seriesWidths.push(nodeTextLength + 28); // 28 is ~ the width of the circle plus some padding
                    });

                    var seriesPerRow = 0;
                    var legendWidth = 0;
                    var columnWidths = [];

                    while (legendWidth < availableWidth && seriesPerRow < seriesWidths.length) {
                        columnWidths[seriesPerRow] = seriesWidths[seriesPerRow];
                        legendWidth += seriesWidths[seriesPerRow++];
                    }
                    if (seriesPerRow === 0) seriesPerRow = 1; //minimum of one series per row


                    while (legendWidth > availableWidth && seriesPerRow > 1) {
                        columnWidths = [];
                        seriesPerRow--;

                        for (var k = 0; k < seriesWidths.length; k++) {
                            if (seriesWidths[k] > (columnWidths[k % seriesPerRow] || 0))
                                columnWidths[k % seriesPerRow] = seriesWidths[k];
                        }

                        legendWidth = columnWidths.reduce(function (prev, cur, index, array) {
                            return prev + cur;
                        });
                    }

                    var xPositions = [];
                    for (var i = 0, curX = 0; i < seriesPerRow; i++) {
                        xPositions[i] = curX;
                        curX += columnWidths[i];
                    }

                    series
                        .attr('transform', function (d, i) {
                            return 'translate(' + xPositions[i % seriesPerRow] + ',' + (5 + Math.floor(i / seriesPerRow) * 20) + ')';
                        });

                    //position legend as far right as possible within the total width
                    if (rightAlign) {
                        g.attr('transform', 'translate(' + (width - margin.right - legendWidth) + ',' + margin.top + ')');
                    }
                    else {
                        g.attr('transform', 'translate(0' + ',' + margin.top + ')');
                    }

                    height = margin.top + margin.bottom + (Math.ceil(seriesWidths.length / seriesPerRow) * 20);

                } else {

                    var ypos = 5,
                        newxpos = 5,
                        maxwidth = 0,
                        xpos;
                    series
                        .attr('transform', function (d, i) {
                            var length = d3.select(this).select('text').node().getComputedTextLength() + 28;
                            xpos = newxpos;

                            if (width < margin.left + margin.right + xpos + length) {
                                newxpos = xpos = 5;
                                ypos += 20;
                            }

                            newxpos += length;
                            if (newxpos > maxwidth) maxwidth = newxpos;

                            return 'translate(' + xpos + ',' + ypos + ')';
                        });

                    //position legend as far right as possible within the total width
                    g.attr('transform', 'translate(' + (width - margin.right - maxwidth) + ',' + margin.top + ')');

                    height = margin.top + margin.bottom + ypos + 15;

                }

            });

            return chart;
        }


        //============================================================
        // Expose Public Variables
        //------------------------------------------------------------

        chart.dispatch = dispatch;
        chart.options = nv.utils.optionsFunc.bind(chart);

        chart.margin = function (_) {
            if (!arguments.length) return margin;
            margin.top = typeof _.top != 'undefined' ? _.top : margin.top;
            margin.right = typeof _.right != 'undefined' ? _.right : margin.right;
            margin.bottom = typeof _.bottom != 'undefined' ? _.bottom : margin.bottom;
            margin.left = typeof _.left != 'undefined' ? _.left : margin.left;
            return chart;
        };

        chart.width = function (_) {
            if (!arguments.length) return width;
            width = _;
            return chart;
        };

        chart.height = function (_) {
            if (!arguments.length) return height;
            height = _;
            return chart;
        };

        chart.key = function (_) {
            if (!arguments.length) return getKey;
            getKey = _;
            return chart;
        };

        chart.color = function (_) {
            if (!arguments.length) return color;
            color = nv.utils.getColor(_);
            return chart;
        };

        chart.align = function (_) {
            if (!arguments.length) return align;
            align = _;
            return chart;
        };

        chart.rightAlign = function (_) {
            if (!arguments.length) return rightAlign;
            rightAlign = _;
            return chart;
        };

        chart.updateState = function (_) {
            if (!arguments.length) return updateState;
            updateState = _;
            return chart;
        };

        chart.radioButtonMode = function (_) {
            if (!arguments.length) return radioButtonMode;
            radioButtonMode = _;
            return chart;
        };

        //============================================================


        return chart;
    }

    nv.models.pie = function () {
        "use strict";
        //============================================================
        // Public Variables with Default Settings
        //------------------------------------------------------------

        var margin = { top: 0, right: 0, bottom: 0, left: 0 }
          , width = 500
          , height = 500
          , getX = function (d) { return d.x }
          , getY = function (d) { return d.y }
          , getDescription = function (d) { return d.description }
          , id = Math.floor(Math.random() * 10000) //Create semi-unique ID in case user doesn't select one
          , color = nv.utils.defaultColor()
          , valueFormat = d3.format(',.2f')
          , labelFormat = d3.format('%')
          , showLabels = true
          , pieLabelsOutside = true
          , donutLabelsOutside = false
          , labelType = "key"
          , labelThreshold = .02 //if slice percentage is under this, don't show label
          , donut = false
          , labelSunbeamLayout = false
          , startAngle = false
          , endAngle = false
          , donutRatio = 0.5
          , dispatch = d3.dispatch('chartClick', 'elementClick', 'elementDblClick', 'elementMouseover', 'elementMouseout')
        ;

        //============================================================

        function chart(selection) {
            selection.each(function (data) {
                var availableWidth = width - margin.left - margin.right,
                    availableHeight = height - margin.top - margin.bottom,
                    radius = Math.min(availableWidth, availableHeight) / 2,
                    arcRadius = radius - (radius / 5),
                    container = d3.select(this);


                //------------------------------------------------------------
                // Setup containers and skeleton of chart

                //var wrap = container.selectAll('.nv-wrap.nv-pie').data([data]);
                var wrap = container.selectAll('.nv-wrap.nv-pie').data(data);
                var wrapEnter = wrap.enter().append('g').attr('class', 'nvd3 nv-wrap nv-pie nv-chart-' + id);
                var gEnter = wrapEnter.append('g');
                var g = wrap.select('g');

                gEnter.append('g').attr('class', 'nv-pie');
                gEnter.append('g').attr('class', 'nv-pieLabels');

                wrap.attr('transform', 'translate(' + margin.left + ',' + margin.top + ')');
                g.select('.nv-pie').attr('transform', 'translate(' + availableWidth / 2 + ',' + availableHeight / 2 + ')');
                g.select('.nv-pieLabels').attr('transform', 'translate(' + availableWidth / 2 + ',' + availableHeight / 2 + ')');

                //------------------------------------------------------------


                container
                    .on('click', function (d, i) {
                        dispatch.chartClick({
                            data: d,
                            index: i,
                            pos: d3.event,
                            id: id
                        });
                    });


                var arc = d3.svg.arc()
                            .outerRadius(arcRadius);

                if (startAngle) arc.startAngle(startAngle)
                if (endAngle) arc.endAngle(endAngle);
                if (donut) arc.innerRadius(radius * donutRatio);

                // Setup the Pie chart and choose the data element
                var pie = d3.layout.pie()
                    .sort(null)
                    .value(function (d) { return d.disabled ? 0 : getY(d) });

                var slices = wrap.select('.nv-pie').selectAll('.nv-slice')
                    .data(pie);

                var pieLabels = wrap.select('.nv-pieLabels').selectAll('.nv-label')
                    .data(pie);

                slices.exit().remove();
                pieLabels.exit().remove();

                var ae = slices.enter().append('g')
                        .attr('class', 'nv-slice')
                        .on('mouseover', function (d, i) {
                            d3.select(this).classed('hover', true);
                            dispatch.elementMouseover({
                                label: getX(d.data),
                                value: getY(d.data),
                                point: d.data,
                                pointIndex: i,
                                pos: [d3.event.pageX, d3.event.pageY],
                                id: id
                            });
                        })
                        .on('mouseout', function (d, i) {
                            d3.select(this).classed('hover', false);
                            dispatch.elementMouseout({
                                label: getX(d.data),
                                value: getY(d.data),
                                point: d.data,
                                index: i,
                                id: id
                            });
                        })
                        .on('click', function (d, i) {
                            dispatch.elementClick({
                                label: getX(d.data),
                                value: getY(d.data),
                                point: d.data,
                                index: i,
                                pos: d3.event,
                                id: id
                            });
                            d3.event.stopPropagation();
                        })
                        .on('dblclick', function (d, i) {
                            dispatch.elementDblClick({
                                label: getX(d.data),
                                value: getY(d.data),
                                point: d.data,
                                index: i,
                                pos: d3.event,
                                id: id
                            });
                            d3.event.stopPropagation();
                        });

                slices
                    .attr('fill', function (d, i) { return color(d, i); })
                    .attr('stroke', function (d, i) { return color(d, i); });

                var paths = ae.append('path')
                    .each(function (d) { this._current = d; });
                //.attr('d', arc);

                slices.select('path')
                  .transition()
                    .attr('d', arc)
                    .attrTween('d', arcTween);

                if (showLabels) {
                    // This does the normal label
                    var labelsArc = d3.svg.arc().innerRadius(0);

                    if (pieLabelsOutside) { labelsArc = arc; }

                    if (donutLabelsOutside) { labelsArc = d3.svg.arc().outerRadius(arc.outerRadius()); }

                    pieLabels.enter().append("g").classed("nv-label", true)
                      .each(function (d, i) {
                          var group = d3.select(this);

                          group
                            .attr('transform', function (d) {
                                if (labelSunbeamLayout) {
                                    d.outerRadius = arcRadius + 10; // Set Outer Coordinate
                                    d.innerRadius = arcRadius + 15; // Set Inner Coordinate
                                    var rotateAngle = (d.startAngle + d.endAngle) / 2 * (180 / Math.PI);
                                    if ((d.startAngle + d.endAngle) / 2 < Math.PI) {
                                        rotateAngle -= 90;
                                    } else {
                                        rotateAngle += 90;
                                    }
                                    return 'translate(' + labelsArc.centroid(d) + ') rotate(' + rotateAngle + ')';
                                } else {
                                    d.outerRadius = radius + 10; // Set Outer Coordinate
                                    d.innerRadius = radius + 15; // Set Inner Coordinate
                                    return 'translate(' + labelsArc.centroid(d) + ')'
                                }
                            });

                          group.append('rect')
                              .style('stroke', '#fff')
                              .style('fill', '#fff')
                              .attr("rx", 3)
                              .attr("ry", 3);

                          group.append('text')
                              .style('text-anchor', labelSunbeamLayout ? ((d.startAngle + d.endAngle) / 2 < Math.PI ? 'start' : 'end') : 'middle') //center the text on it's origin or begin/end if orthogonal aligned
                              .style('fill', '#000')

                      });

                    var labelLocationHash = {};
                    var avgHeight = 14;
                    var avgWidth = 140;
                    var createHashKey = function (coordinates) {

                        return Math.floor(coordinates[0] / avgWidth) * avgWidth + ',' + Math.floor(coordinates[1] / avgHeight) * avgHeight;
                    };
                    pieLabels.transition()
                          .attr('transform', function (d) {
                              if (labelSunbeamLayout) {
                                  d.outerRadius = arcRadius + 10; // Set Outer Coordinate
                                  d.innerRadius = arcRadius + 15; // Set Inner Coordinate
                                  var rotateAngle = (d.startAngle + d.endAngle) / 2 * (180 / Math.PI);
                                  if ((d.startAngle + d.endAngle) / 2 < Math.PI) {
                                      rotateAngle -= 90;
                                  } else {
                                      rotateAngle += 90;
                                  }
                                  return 'translate(' + labelsArc.centroid(d) + ') rotate(' + rotateAngle + ')';
                              } else {
                                  d.outerRadius = radius + 10; // Set Outer Coordinate
                                  d.innerRadius = radius + 15; // Set Inner Coordinate

                                  /*
                                  Overlapping pie labels are not good. What this attempts to do is, prevent overlapping.
                                  Each label location is hashed, and if a hash collision occurs, we assume an overlap.
                                  Adjust the label's y-position to remove the overlap.
                                  */
                                  var center = labelsArc.centroid(d);
                                  if (d.value) {
                                      var hashKey = createHashKey(center);
                                      if (labelLocationHash[hashKey]) {
                                          center[1] -= avgHeight;
                                      }
                                      labelLocationHash[createHashKey(center)] = true;
                                  }
                                  return 'translate(' + center + ')'
                              }
                          });
                    pieLabels.select(".nv-label text")
                          .style('text-anchor', labelSunbeamLayout ? ((d.startAngle + d.endAngle) / 2 < Math.PI ? 'start' : 'end') : 'middle') //center the text on it's origin or begin/end if orthogonal aligned
                          .text(function (d, i) {
                              var percent = (d.endAngle - d.startAngle) / (2 * Math.PI);
                              var labelTypes = {
                                  "key": getX(d.data),
                                  "value": getY(d.data),
                                  "percent": labelFormat(percent)
                              };
                              return (d.value && percent > labelThreshold) ? labelTypes[labelType] : '';
                          });
                }


                // Computes the angle of an arc, converting from radians to degrees.
                function angle(d) {
                    var a = (d.startAngle + d.endAngle) * 90 / Math.PI - 90;
                    return a > 90 ? a - 180 : a;
                }

                function arcTween(a) {
                    a.endAngle = isNaN(a.endAngle) ? 0 : a.endAngle;
                    a.startAngle = isNaN(a.startAngle) ? 0 : a.startAngle;
                    if (!donut) a.innerRadius = 0;
                    var i = d3.interpolate(this._current, a);
                    this._current = i(0);
                    return function (t) {
                        return arc(i(t));
                    };
                }

                function tweenPie(b) {
                    b.innerRadius = 0;
                    var i = d3.interpolate({ startAngle: 0, endAngle: 0 }, b);
                    return function (t) {
                        return arc(i(t));
                    };
                }

            });

            return chart;
        }


        //============================================================
        // Expose Public Variables
        //------------------------------------------------------------

        chart.dispatch = dispatch;
        chart.options = nv.utils.optionsFunc.bind(chart);

        chart.margin = function (_) {
            if (!arguments.length) return margin;
            margin.top = typeof _.top != 'undefined' ? _.top : margin.top;
            margin.right = typeof _.right != 'undefined' ? _.right : margin.right;
            margin.bottom = typeof _.bottom != 'undefined' ? _.bottom : margin.bottom;
            margin.left = typeof _.left != 'undefined' ? _.left : margin.left;
            return chart;
        };

        chart.width = function (_) {
            if (!arguments.length) return width;
            width = _;
            return chart;
        };

        chart.height = function (_) {
            if (!arguments.length) return height;
            height = _;
            return chart;
        };

        chart.values = function (_) {
            nv.log("pie.values() is no longer supported.");
            return chart;
        };

        chart.x = function (_) {
            if (!arguments.length) return getX;
            getX = _;
            return chart;
        };

        chart.y = function (_) {
            if (!arguments.length) return getY;
            getY = d3.functor(_);
            return chart;
        };

        chart.description = function (_) {
            if (!arguments.length) return getDescription;
            getDescription = _;
            return chart;
        };

        chart.showLabels = function (_) {
            if (!arguments.length) return showLabels;
            showLabels = _;
            return chart;
        };

        chart.labelSunbeamLayout = function (_) {
            if (!arguments.length) return labelSunbeamLayout;
            labelSunbeamLayout = _;
            return chart;
        };

        chart.donutLabelsOutside = function (_) {
            if (!arguments.length) return donutLabelsOutside;
            donutLabelsOutside = _;
            return chart;
        };

        chart.pieLabelsOutside = function (_) {
            if (!arguments.length) return pieLabelsOutside;
            pieLabelsOutside = _;
            return chart;
        };

        chart.labelType = function (_) {
            if (!arguments.length) return labelType;
            labelType = _;
            labelType = labelType || "key";
            return chart;
        };

        chart.donut = function (_) {
            if (!arguments.length) return donut;
            donut = _;
            return chart;
        };

        chart.donutRatio = function (_) {
            if (!arguments.length) return donutRatio;
            donutRatio = _;
            return chart;
        };

        chart.startAngle = function (_) {
            if (!arguments.length) return startAngle;
            startAngle = _;
            return chart;
        };

        chart.endAngle = function (_) {
            if (!arguments.length) return endAngle;
            endAngle = _;
            return chart;
        };

        chart.id = function (_) {
            if (!arguments.length) return id;
            id = _;
            return chart;
        };

        chart.color = function (_) {
            if (!arguments.length) return color;
            color = nv.utils.getColor(_);
            return chart;
        };

        chart.valueFormat = function (_) {
            if (!arguments.length) return valueFormat;
            valueFormat = _;
            return chart;
        };

        chart.labelFormat = function (_) {
            if (!arguments.length) return labelFormat;
            labelFormat = _;
            return chart;
        };

        chart.labelThreshold = function (_) {
            if (!arguments.length) return labelThreshold;
            labelThreshold = _;
            return chart;
        };
        //============================================================


        return chart;
    }

    nv.models.pieChart = function () {
        "use strict";
        //============================================================
        // Public Variables with Default Settings
        //------------------------------------------------------------

        var pie = nv.models.pie()
          , legend = nv.models.legend()
        ;

        var margin = { top: 30, right: 20, bottom: 20, left: 20 }
          , width = null
          , height = null
          , showLegend = true
          , color = nv.utils.defaultColor()
          , tooltips = true
          , tooltip = function (key, y, e, graph) {
              return '<h3>' + key + '</h3>' +
                     '<p>' + y + '</p>'
          }
          , state = {}
          , defaultState = null
          , noData = "No Data Available."
          , dispatch = d3.dispatch('tooltipShow', 'tooltipHide', 'stateChange', 'changeState')
        ;

        //============================================================


        //============================================================
        // Private Variables
        //------------------------------------------------------------

        var showTooltip = function (e, offsetElement) {
            var tooltipLabel = pie.description()(e.point) || pie.x()(e.point)
            var left = e.pos[0] + ((offsetElement && offsetElement.offsetLeft) || 0),
                top = e.pos[1] + ((offsetElement && offsetElement.offsetTop) || 0),
                y = pie.valueFormat()(pie.y()(e.point)),
                content = tooltip(tooltipLabel, y, e, chart);

            nv.tooltip.show([left, top], content, e.value < 0 ? 'n' : 's', null, offsetElement);
        };

        //============================================================


        function chart(selection) {
            selection.each(function (data) {
                var container = d3.select(this),
                    that = this;

                var availableWidth = (width || parseInt(container.style('width')) || 960)
                                       - margin.left - margin.right,
                    availableHeight = (height || parseInt(container.style('height')) || 400)
                                       - margin.top - margin.bottom;

                chart.update = function () { container.transition().call(chart); };
                chart.container = this;

                //set state.disabled
                state.disabled = data.map(function (d) { return !!d.disabled });

                if (!defaultState) {
                    var key;
                    defaultState = {};
                    for (key in state) {
                        if (state[key] instanceof Array)
                            defaultState[key] = state[key].slice(0);
                        else
                            defaultState[key] = state[key];
                    }
                }

                //------------------------------------------------------------
                // Display No Data message if there's nothing to show.

                if (!data || !data.length) {
                    var noDataText = container.selectAll('.nv-noData').data([noData]);

                    noDataText.enter().append('text')
                      .attr('class', 'nvd3 nv-noData')
                      .attr('dy', '-.7em')
                      .style('text-anchor', 'middle');

                    noDataText
                      .attr('x', margin.left + availableWidth / 2)
                      .attr('y', margin.top + availableHeight / 2)
                      .text(function (d) { return d });

                    return chart;
                } else {
                    container.selectAll('.nv-noData').remove();
                }

                //------------------------------------------------------------
                //------------------------------------------------------------
                // Setup containers and skeleton of chart

                var wrap = container.selectAll('g.nv-wrap.nv-pieChart').data([data]);
                var gEnter = wrap.enter().append('g').attr('class', 'nvd3 nv-wrap nv-pieChart').append('g');
                var g = wrap.select('g');

                gEnter.append('g').attr('class', 'nv-pieWrap');
                gEnter.append('g').attr('class', 'nv-legendWrap');

                //------------------------------------------------------------
                //------------------------------------------------------------
                // Legend

                if (showLegend) {
                    legend
                      .width(180)
                      .key(pie.x());

                    wrap.select('.nv-legendWrap')
                        .datum(data)
                        .call(legend)
                }
                //------------------------------------------------------------

                wrap.attr('transform', 'translate(' + margin.left + ',' + (margin.top) + ')');
                //wrap.select('.nv-legendWrap').attr('transform', 'translate(0,' + (-margin.top / 2) + ')');

                //------------------------------------------------------------
                // Main Chart Component(s)

                pie.width(availableWidth).height(availableHeight);

                var pieWrap = g.select('.nv-pieWrap').datum([data]);

                d3.transition(pieWrap).call(pie);

                //------------------------------------------------------------


                //============================================================
                // Event Handling/Dispatching (in chart's scope)
                //------------------------------------------------------------

                legend.dispatch.on('stateChange', function (newState) {
                    state = newState;
                    dispatch.stateChange(state);
                    chart.update();
                });

                pie.dispatch.on('elementMouseout.tooltip', function (e) {
                    dispatch.tooltipHide(e);
                });

                // Update chart from a state object passed to event handler
                dispatch.on('changeState', function (e) {

                    if (typeof e.disabled !== 'undefined') {
                        data.forEach(function (series, i) {
                            series.disabled = e.disabled[i];
                        });

                        state.disabled = e.disabled;
                    }

                    chart.update();
                });

                //============================================================


            });

            return chart;
        }

        //============================================================
        // Event Handling/Dispatching (out of chart's scope)
        //------------------------------------------------------------

        pie.dispatch.on('elementMouseover.tooltip', function (e) {
            e.pos = [e.pos[0] + margin.left, e.pos[1] + margin.top];
            dispatch.tooltipShow(e);
        });

        dispatch.on('tooltipShow', function (e) {
            if (tooltips) showTooltip(e);
        });

        dispatch.on('tooltipHide', function () {
            if (tooltips) nv.tooltip.cleanup();
        });

        //============================================================


        //============================================================
        // Expose Public Variables
        //------------------------------------------------------------

        // expose chart's sub-components
        chart.legend = legend;
        chart.dispatch = dispatch;
        chart.pie = pie;

        d3.rebind(chart, pie, 'valueFormat', 'labelFormat', 'values', 'x', 'y', 'description', 'id', 'showLabels', 'donutLabelsOutside', 'pieLabelsOutside', 'labelType', 'donut', 'donutRatio', 'labelThreshold');
        chart.options = nv.utils.optionsFunc.bind(chart);

        chart.margin = function (_) {
            if (!arguments.length) return margin;
            margin.top = typeof _.top != 'undefined' ? _.top : margin.top;
            margin.right = typeof _.right != 'undefined' ? _.right : margin.right;
            margin.bottom = typeof _.bottom != 'undefined' ? _.bottom : margin.bottom;
            margin.left = typeof _.left != 'undefined' ? _.left : margin.left;
            return chart;
        };

        chart.width = function (_) {
            if (!arguments.length) return width;
            width = _;
            return chart;
        };

        chart.height = function (_) {
            if (!arguments.length) return height;
            height = _;
            return chart;
        };

        chart.color = function (_) {
            if (!arguments.length) return color;
            color = nv.utils.getColor(_);
            legend.color(color);
            pie.color(color);
            return chart;
        };

        chart.showLegend = function (_) {
            if (!arguments.length) return showLegend;
            showLegend = _;
            return chart;
        };

        chart.tooltips = function (_) {
            if (!arguments.length) return tooltips;
            tooltips = _;
            return chart;
        };

        chart.tooltipContent = function (_) {
            if (!arguments.length) return tooltip;
            tooltip = _;
            return chart;
        };

        chart.state = function (_) {
            if (!arguments.length) return state;
            state = _;
            return chart;
        };

        chart.defaultState = function (_) {
            if (!arguments.length) return defaultState;
            defaultState = _;
            return chart;
        };

        chart.noData = function (_) {
            if (!arguments.length) return noData;
            noData = _;
            return chart;
        };

        //============================================================


        return chart;
    }

    window.nv = nv;
})();

(function () {
    "use strict";
    window.nv.tooltip = {};

    /* Model which can be instantiated to handle tooltip rendering.
      Example usage: 
      var tip = nv.models.tooltip().gravity('w').distance(23)
                  .data(myDataObject);
  
          tip();    //just invoke the returned function to render tooltip.
    */
    window.nv.models.tooltip = function () {
        var content = null    //HTML contents of the tooltip.  If null, the content is generated via the data variable.
        , data = null     //Tooltip data. If data is given in the proper format, a consistent tooltip is generated.
        , gravity = 'w'   //Can be 'n','s','e','w'. Determines how tooltip is positioned.
        , distance = 50   //Distance to offset tooltip from the mouse location.
        , snapDistance = 25   //Tolerance allowed before tooltip is moved from its current position (creates 'snapping' effect)
        , fixedTop = null //If not null, this fixes the top position of the tooltip.
        , classes = null  //Attaches additional CSS classes to the tooltip DIV that is created.
        , chartContainer = null   //Parent DIV, of the SVG Container that holds the chart.
        , tooltipElem = null  //actual DOM element representing the tooltip.
        , position = { left: null, top: null }      //Relative position of the tooltip inside chartContainer.
        , enabled = true  //True -> tooltips are rendered. False -> don't render tooltips.
        //Generates a unique id when you create a new tooltip() object
        , id = "nvtooltip-" + Math.floor(Math.random() * 100000);

        //CSS class to specify whether element should not have mouse events.
        var nvPointerEventsClass = "nv-pointer-events-none";

        //Format function for the tooltip values column
        var valueFormatter = function (d, i) {
            return d;
        };

        //Format function for the tooltip header value.
        var headerFormatter = function (d) {
            return d;
        };

        //By default, the tooltip model renders a beautiful table inside a DIV.
        //You can override this function if a custom tooltip is desired.
        var contentGenerator = function (d) {
            if (content != null) return content;

            if (d == null) return '';

            var table = d3.select(document.createElement("table"));
            var theadEnter = table.selectAll("thead")
                .data([d])
                .enter().append("thead");
            theadEnter.append("tr")
                .append("td")
                .attr("colspan", 3)
                .append("strong")
                    .classed("x-value", true)
                    .html(headerFormatter(d.value));

            var tbodyEnter = table.selectAll("tbody")
                .data([d])
                .enter().append("tbody");
            var trowEnter = tbodyEnter.selectAll("tr")
                .data(function (p) { return p.series })
                .enter()
                .append("tr")
                .classed("highlight", function (p) { return p.highlight })
            ;

            trowEnter.append("td")
                .classed("legend-color-guide", true)
                .append("div")
                .style("background-color", function (p) { return p.color });
            trowEnter.append("td")
                .classed("key", true)
                .html(function (p) { return p.key });
            trowEnter.append("td")
                .classed("value", true)
                .html(function (p, i) { return valueFormatter(p.value, i) });


            trowEnter.selectAll("td").each(function (p) {
                if (p.highlight) {
                    var opacityScale = d3.scale.linear().domain([0, 1]).range(["#fff", p.color]);
                    var opacity = 0.6;
                    d3.select(this)
                        .style("border-bottom-color", opacityScale(opacity))
                        .style("border-top-color", opacityScale(opacity))
                    ;
                }
            });

            var html = table.node().outerHTML;
            if (d.footer !== undefined)
                html += "<div class='footer'>" + d.footer + "</div>";
            return html;

        };

        var dataSeriesExists = function (d) {
            if (d && d.series && d.series.length > 0) return true;

            return false;
        };

        //In situations where the chart is in a 'viewBox', re-position the tooltip based on how far chart is zoomed.
        function convertViewBoxRatio() {
            if (chartContainer) {
                var svg = d3.select(chartContainer);
                if (svg.node().tagName !== "svg") {
                    svg = svg.select("svg");
                }
                var viewBox = (svg.node()) ? svg.attr('viewBox') : null;
                if (viewBox) {
                    viewBox = viewBox.split(' ');
                    var ratio = parseInt(svg.style('width')) / viewBox[2];

                    position.left = position.left * ratio;
                    position.top = position.top * ratio;
                }
            }
        }

        //Creates new tooltip container, or uses existing one on DOM.
        function getTooltipContainer(newContent) {
            var body;
            if (chartContainer)
                body = d3.select(chartContainer);
            else
                body = d3.select("body");

            var container = body.select(".nvtooltip");
            if (container.node() === null) {
                //Create new tooltip div if it doesn't exist on DOM.
                container = body.append("div")
                    .attr("class", "nvtooltip " + (classes ? classes : "xy-tooltip"))
                    .attr("id", id)
                ;
            }


            container.node().innerHTML = newContent;
            container.style("top", 0).style("left", 0).style("opacity", 0);
            container.selectAll("div, table, td, tr").classed(nvPointerEventsClass, true)
            container.classed(nvPointerEventsClass, true);
            return container.node();
        }



        //Draw the tooltip onto the DOM.
        function nvtooltip() {
            if (!enabled) return;
            if (!dataSeriesExists(data)) return;

            convertViewBoxRatio();

            var left = position.left;
            var top = (fixedTop != null) ? fixedTop : position.top;
            var container = getTooltipContainer(contentGenerator(data));
            tooltipElem = container;
            if (chartContainer) {
                var svgComp = chartContainer.getElementsByTagName("svg")[0];
                var boundRect = (svgComp) ? svgComp.getBoundingClientRect() : chartContainer.getBoundingClientRect();
                var svgOffset = { left: 0, top: 0 };
                if (svgComp) {
                    var svgBound = svgComp.getBoundingClientRect();
                    var chartBound = chartContainer.getBoundingClientRect();
                    var svgBoundTop = svgBound.top;

                    //Defensive code. Sometimes, svgBoundTop can be a really negative
                    //  number, like -134254. That's a bug. 
                    //  If such a number is found, use zero instead. FireFox bug only
                    if (svgBoundTop < 0) {
                        var containerBound = chartContainer.getBoundingClientRect();
                        svgBoundTop = (Math.abs(svgBoundTop) > containerBound.height) ? 0 : svgBoundTop;
                    }
                    svgOffset.top = Math.abs(svgBoundTop - chartBound.top);
                    svgOffset.left = Math.abs(svgBound.left - chartBound.left);
                }
                //If the parent container is an overflow <div> with scrollbars, subtract the scroll offsets.
                //You need to also add any offset between the <svg> element and its containing <div>
                //Finally, add any offset of the containing <div> on the whole page.
                left += chartContainer.offsetLeft + svgOffset.left - 2 * chartContainer.scrollLeft;
                top += chartContainer.offsetTop + svgOffset.top - 2 * chartContainer.scrollTop;
            }

            if (snapDistance && snapDistance > 0) {
                top = Math.floor(top / snapDistance) * snapDistance;
            }

            nv.tooltip.calcTooltipPosition([left, top], gravity, distance, container);
            return nvtooltip;
        };

        nvtooltip.nvPointerEventsClass = nvPointerEventsClass;

        nvtooltip.content = function (_) {
            if (!arguments.length) return content;
            content = _;
            return nvtooltip;
        };

        //Returns tooltipElem...not able to set it.
        nvtooltip.tooltipElem = function () {
            return tooltipElem;
        };

        nvtooltip.contentGenerator = function (_) {
            if (!arguments.length) return contentGenerator;
            if (typeof _ === 'function') {
                contentGenerator = _;
            }
            return nvtooltip;
        };

        nvtooltip.data = function (_) {
            if (!arguments.length) return data;
            data = _;
            return nvtooltip;
        };

        nvtooltip.gravity = function (_) {
            if (!arguments.length) return gravity;
            gravity = _;
            return nvtooltip;
        };

        nvtooltip.distance = function (_) {
            if (!arguments.length) return distance;
            distance = _;
            return nvtooltip;
        };

        nvtooltip.snapDistance = function (_) {
            if (!arguments.length) return snapDistance;
            snapDistance = _;
            return nvtooltip;
        };

        nvtooltip.classes = function (_) {
            if (!arguments.length) return classes;
            classes = _;
            return nvtooltip;
        };

        nvtooltip.chartContainer = function (_) {
            if (!arguments.length) return chartContainer;
            chartContainer = _;
            return nvtooltip;
        };

        nvtooltip.position = function (_) {
            if (!arguments.length) return position;
            position.left = (typeof _.left !== 'undefined') ? _.left : position.left;
            position.top = (typeof _.top !== 'undefined') ? _.top : position.top;
            return nvtooltip;
        };

        nvtooltip.fixedTop = function (_) {
            if (!arguments.length) return fixedTop;
            fixedTop = _;
            return nvtooltip;
        };

        nvtooltip.enabled = function (_) {
            if (!arguments.length) return enabled;
            enabled = _;
            return nvtooltip;
        };

        nvtooltip.valueFormatter = function (_) {
            if (!arguments.length) return valueFormatter;
            if (typeof _ === 'function') {
                valueFormatter = _;
            }
            return nvtooltip;
        };

        nvtooltip.headerFormatter = function (_) {
            if (!arguments.length) return headerFormatter;
            if (typeof _ === 'function') {
                headerFormatter = _;
            }
            return nvtooltip;
        };

        //id() is a read-only function. You can't use it to set the id.
        nvtooltip.id = function () {
            return id;
        };


        return nvtooltip;
    };


    //Original tooltip.show function. Kept for backward compatibility.
    // pos = [left,top]
    nv.tooltip.show = function (pos, content, gravity, dist, parentContainer, classes) {

        //Create new tooltip div if it doesn't exist on DOM.
        var container = document.createElement('div');
        container.className = 'nvtooltip ' + (classes ? classes : 'xy-tooltip');

        var body = parentContainer;
        if (!parentContainer || parentContainer.tagName.match(/g|svg/i)) {
            //If the parent element is an SVG element, place tooltip in the <body> element.
            body = document.getElementsByTagName('body')[0];
        }

        container.style.left = 0;
        container.style.top = 0;
        container.style.opacity = 0;
        container.innerHTML = content;
        body.appendChild(container);

        //If the parent container is an overflow <div> with scrollbars, subtract the scroll offsets.
        if (parentContainer) {
            pos[0] = pos[0] - parentContainer.scrollLeft;
            pos[1] = pos[1] - parentContainer.scrollTop;
        }
        nv.tooltip.calcTooltipPosition(pos, gravity, dist, container);
    };

    //Looks up the ancestry of a DOM element, and returns the first NON-svg node.
    nv.tooltip.findFirstNonSVGParent = function (Elem) {
        while (Elem.tagName.match(/^g|svg$/i) !== null) {
            Elem = Elem.parentNode;
        }
        return Elem;
    };

    //Finds the total offsetTop of a given DOM element.
    //Looks up the entire ancestry of an element, up to the first relatively positioned element.
    nv.tooltip.findTotalOffsetTop = function (Elem, initialTop) {
        var offsetTop = initialTop;

        do {
            if (!isNaN(Elem.offsetTop)) {
                offsetTop += (Elem.offsetTop);
            }
        } while (Elem = Elem.offsetParent);
        return offsetTop;
    };

    //Finds the total offsetLeft of a given DOM element.
    //Looks up the entire ancestry of an element, up to the first relatively positioned element.
    nv.tooltip.findTotalOffsetLeft = function (Elem, initialLeft) {
        var offsetLeft = initialLeft;

        do {
            if (!isNaN(Elem.offsetLeft)) {
                offsetLeft += (Elem.offsetLeft);
            }
        } while (Elem = Elem.offsetParent);
        return offsetLeft;
    };

    //Global utility function to render a tooltip on the DOM.
    //pos = [left,top] coordinates of where to place the tooltip, relative to the SVG chart container.
    //gravity = how to orient the tooltip
    //dist = how far away from the mouse to place tooltip
    //container = tooltip DIV
    nv.tooltip.calcTooltipPosition = function (pos, gravity, dist, container) {

        var height = parseInt(container.offsetHeight),
            width = parseInt(container.offsetWidth),
            windowWidth = nv.utils.windowSize().width,
            windowHeight = nv.utils.windowSize().height,
            scrollTop = window.pageYOffset,
            scrollLeft = window.pageXOffset,
            left, top;

        windowHeight = window.innerWidth >= document.body.scrollWidth ? windowHeight : windowHeight - 16;
        windowWidth = window.innerHeight >= document.body.scrollHeight ? windowWidth : windowWidth - 16;

        gravity = gravity || 's';
        dist = dist || 20;

        var tooltipTop = function (Elem) {
            return nv.tooltip.findTotalOffsetTop(Elem, top);
        };

        var tooltipLeft = function (Elem) {
            return nv.tooltip.findTotalOffsetLeft(Elem, left);
        };

        switch (gravity) {
            case 'e':
                left = pos[0] - width - dist;
                top = pos[1] - (height / 2);
                var tLeft = tooltipLeft(container);
                var tTop = tooltipTop(container);
                if (tLeft < scrollLeft) left = pos[0] + dist > scrollLeft ? pos[0] + dist : scrollLeft - tLeft + left;
                if (tTop < scrollTop) top = scrollTop - tTop + top;
                if (tTop + height > scrollTop + windowHeight) top = scrollTop + windowHeight - tTop + top - height;
                break;
            case 'w':
                left = pos[0] + dist;
                top = pos[1] - (height / 2);
                var tLeft = tooltipLeft(container);
                var tTop = tooltipTop(container);
                if (tLeft + width > windowWidth) left = pos[0] - width - dist;
                if (tTop < scrollTop) top = scrollTop + 5;
                if (tTop + height > scrollTop + windowHeight) top = scrollTop + windowHeight - tTop + top - height;
                break;
            case 'n':
                left = pos[0] - (width / 2) - 5;
                top = pos[1] + dist;
                var tLeft = tooltipLeft(container);
                var tTop = tooltipTop(container);
                if (tLeft < scrollLeft) left = scrollLeft + 5;
                if (tLeft + width > windowWidth) left = left - width / 2 + 5;
                if (tTop + height > scrollTop + windowHeight) top = scrollTop + windowHeight - tTop + top - height;
                break;
            case 's':
                left = pos[0] - (width / 2);
                top = pos[1] - height - dist;
                var tLeft = tooltipLeft(container);
                var tTop = tooltipTop(container);
                if (tLeft < scrollLeft) left = scrollLeft + 5;
                if (tLeft + width > windowWidth) left = left - width / 2 + 5;
                if (scrollTop > tTop) top = scrollTop;
                break;
            case 'none':
                left = pos[0];
                top = pos[1] - dist;
                var tLeft = tooltipLeft(container);
                var tTop = tooltipTop(container);
                break;
        }


        container.style.left = left + 'px';
        container.style.top = top + 'px';
        container.style.opacity = 1;
        container.style.position = 'absolute';

        return container;
    };

    //Global utility function to remove tooltips from the DOM.
    nv.tooltip.cleanup = function () {

        // Find the tooltips, mark them for removal by this class (so others cleanups won't find it)
        var tooltips = document.getElementsByClassName('nvtooltip');
        var purging = [];
        while (tooltips.length) {
            purging.push(tooltips[0]);
            tooltips[0].style.transitionDelay = '0 !important';
            tooltips[0].style.opacity = 0;
            tooltips[0].className = 'nvtooltip-pending-removal';
        }

        setTimeout(function () {

            while (purging.length) {
                var removeMe = purging.pop();
                removeMe.parentNode.removeChild(removeMe);
            }
        }, 500);
    };

})();
