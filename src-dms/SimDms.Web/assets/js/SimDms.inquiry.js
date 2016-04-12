SimDms.Inquiry = function (options) {
    var selector = SimDms.selector + " .main";

    this.render = function (callback) {
        var _this = this;
        var content = $(selector);
        var widget = new SimDms.Widget();
        var html_title = ((options.title === undefined) ? "" : "<h3>" + options.title + "</h3>");
        var html_filter = widget.generatePanel(options.panels[0], "<div class=\"subtitle\">Filter</div>");
        var html_grid = "<div class=\"panel kgrid\">"
                      + "<div class=\"subtitle\">Data</div>"
                      + "<div id=\"clientsDb\"><div id=\"" + options.panels[1].name + "\"></div></div>"
                      + "</div>";

        $(".page .title h3").html(options.title);
        content.html("<div class=\"gl-widget\">" + html_filter + html_grid + "</div>");

        widget.post("ab.api/inquiry/employees", function (data) {
            $(".kgrid #" + options.panels[1].name).kendoGrid({
                dataSource: {
                    data: data,
                    pageSize: (options.panels[1].pageSize || 10)
                },
                pageable: {
                    refresh: false,
                    pageSizes: true
                },
                groupable: (options.columns || false),
                sortable: (options.sortable || true),
                filterable: (options.filterable || false),
                columns: options.panels[1].columns
            });
        })
    }


    var firstNames = ["Nancy", "Andrew", "Janet", "Margaret", "Steven", "Mi chael", "Robert", "Laura", "Anne", "Nige"],
        lastNames = ["Davolio", "Fuller", "Leverling", "Peacock", "Buchanan", "Suyama", "King", "Callahan", "Dodsworth", "White"],
        cities = ["Seattle", "Tacoma", "Kirkland", "Redmond", "London", "Philadelphia", "New York", "Seattle", "London", "Boston"],
        titles = ["Accountant", "Vice President, Sales", "Sales Representative", "Technical Support", "Sales Manager", "Web Designer",
        "Software Developer", "Inside Sales Coordinator", "Chief Techical Officer", "Chief Execute Officer"],
        birthDates = [new Date("1948/12/08"), new Date("1952/02/19"), new Date("1963/08/30"), new Date("1937/09/19"), new Date("1955/03/04"), new Date("1963/07/02"), new Date("1960/05/29"), new Date("1958/01/09"), new Date("1966/01/27"), new Date("1966/03/27")];

    this.createRandomData = function (count) {
        var data = [],
            now = new Date();
        for (var i = 0; i < count; i++) {
            var firstName = firstNames[Math.floor(Math.random() * firstNames.length)],
                lastName = lastNames[Math.floor(Math.random() * lastNames.length)],
                city = cities[Math.floor(Math.random() * cities.length)],
                title = titles[Math.floor(Math.random() * titles.length)],
                birthDate = birthDates[Math.floor(Math.random() * birthDates.length)],
                age = now.getFullYear() - birthDate.getFullYear();

            data.push({
                EmployeeID: i + 1,
                EmployeeName: firstName,
                Department: lastName,
                Position: city,
                GradeName: title,
                JoinDate: birthDate
            });
        }
        return data;
    }

    this.generatePeople = function (itemCount, callback) {
        var data = [],
            delay = 25,
            interval = 500,
            starttime;

        var now = new Date();
        setTimeout(function () {
            starttime = +new Date();
            do {
                var firstName = firstNames[Math.floor(Math.random() * firstNames.length)],
                    lastName = lastNames[Math.floor(Math.random() * lastNames.length)],
                    city = cities[Math.floor(Math.random() * cities.length)],
                    title = titles[Math.floor(Math.random() * titles.length)],
                    birthDate = birthDates[Math.floor(Math.random() * birthDates.length)],
                    age = now.getFullYear() - birthDate.getFullYear();

                data.push({
                    Id: data.length + 1,
                    FirstName: firstName,
                    LastName: lastName,
                    City: city,
                    Title: title,
                    BirthDate: birthDate,
                    Age: age
                });
            } while (data.length < itemCount && +new Date() - starttime < interval);

            if (data.length < itemCount) {
                setTimeout(arguments.callee, delay);
            } else {
                callback(data);
            }
        }, delay);
    }
}