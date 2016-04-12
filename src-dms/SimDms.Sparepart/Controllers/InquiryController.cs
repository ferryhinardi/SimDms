using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class InquiryController:BaseController
    {
        //  Inquiry Data Pemasok
        public string lnk8001()
        {
            return HtmlRender("inquiry/InquiryDataPemasok.js");
        }


        //  Inquiry Data Pelanggan
        public string lnk8002()
        {
            return HtmlRender("inquiry/InquiryDataPelanggan.js");
        }

        //  Inquiry Item Sparepart
        public string lnk8003()
        {
            return HtmlRender("inquiry/InquiryItemSparepart.js");
        }

        //  Inquiry Item Sparepart (HQ)
        public string lnk8004()
        {
            return HtmlRender("inquiry/InquiryItemSparepartHQ.js");
        }

        //  Inquiry Item Spareparts (SIMPLE)
        public string lnk8005()
        {
            return HtmlRender("inquiry/InquiryItemSparepartsSIMPLE.js");
        }

        //  Inquiry Penerimaan Barang
        public string lnk8006()
        {
            return HtmlRender("inquiry/InquiryPenerimaanBarang.js");
        }

        //  Inquiry Sales Order
        public string lnk8007()
        {
            return HtmlRender("inquiry/InquirySalesOrder.js");
        }

        //  Inquiry Supply Slip
        public string lnk8008()
        {
            return HtmlRender("inquiry/InquirySupplySlip.js");
        }


        /// <summary>
        ///   The <c>DocumentationSample</c> type 
        ///   demonstrates code comments.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The <c>DocumentationSample</c> type 
        ///     provides no real functionality; 
        ///     however, it does provide examples of 
        ///     using the most common, built in 
        ///     <c>C#</c> xml documentation tags.
        ///   </para>
        ///   <para>
        ///     <c>DocumentationSample</c> types are not 
        ///      safe for concurrent access by 
        ///     multiple threads.
        ///   </para>
        /// </remarks>
        public string lnk8009()
        {
            return HtmlRender("inquiry/InquiryDataDCS.js");
        }

        /// <summary>
        ///   Initializes a new instance of a 
        ///   <c>DocumentationSample</c> type.
        /// </summary>
        /// <example>The following is an example of initializing a 
        /// <c>DocumentationSample</c> type:
        ///   <code>
        ///     // Create the type.
        ///     DocumentationSample ds = new DocumentationSample();
        ///     
        ///     if ( null == ds )
        ///       return;
        ///       
        ///     return ds.MyMethod( “someString” );
        ///   </code>
        /// </example>
        public string lnk8010()
        {
            return HtmlRender("inquiry/InquiryDataSparePart.js");
        }

        /// <summary>Causes something happen.</summary>
        /// <param name="someValue">
        ///   A <see cref="String"/> type representing a value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   if <paramref name="someValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   if <paramref name="someValue"/> is <c>empty</c>.
        /// </exception>
        /// <returns><paramref name="someValue"/> 
        ///   as passed in.</returns>
        public string lnk8011()
        {
            return HtmlRender("inquiry/InquiryDataPartSales.js");
        }

    }

}
