using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLK.Web.Mvc
{
    public sealed class Pagination
    {
        // Fields
        private object _actionModel = null;

        private int _currentPageIndex = 1;

        private int _itemCount = 0;

        private int _pageSize = 10;

        private string _actionModelName = "";

        private string _pageIndexName = "pageIndex";

        private string _itemCountName = "itemCount";

        private string _pageSizeName = "pageSize";

        private int? _pageCount = null;


        // Constructors
        public Pagination(object actionModel = null, int currentPageIndex = 1, int itemCount = 0, int pageSize = 10,
                          string actionModelName = "",
                          string pageIndexName = "pageIndex",
                          string itemCountName = "itemCount",
                          string pageSizeName = "pageSize")
        {
            // Arguments
            this.ActionModel = actionModel;
            this.CurrentPageIndex = currentPageIndex;
            this.ItemCount = itemCount;
            this.PageSize = pageSize;
            this.ActionModelName = actionModelName;
            this.PageIndexName = pageIndexName;
            this.ItemCountName = ItemCountName;
            this.PageSizeName = pageSizeName;
        }


        // Properties
        public object ActionModel
        {
            get { return _actionModel; }
            set { _actionModel = value; }
        }

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set { _currentPageIndex = value; }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set { _itemCount = value; _pageCount = null; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; _pageCount = null; }
        }

        public string ActionModelName
        {
            get { return _actionModelName; }
            set { _actionModelName = value; }
        }

        public string PageIndexName
        {
            get { return _pageIndexName; }
            set { _pageIndexName = value; }
        }

        public string ItemCountName
        {
            get { return _itemCountName; }
            set { _itemCountName = value; }
        }

        public string PageSizeName
        {
            get { return _pageSizeName; }
            set { _pageSizeName = value; }
        }

        public int PageCount
        {
            get
            {
                if (_pageCount.HasValue == false)
                {
                    _pageCount = this.ItemCount / this.PageSize;
                    if (this.ItemCount % this.PageSize != 0) _pageCount++;
                }
                return _pageCount.Value;
            }
        }


        // Methods
        public string CreateHttpUrl(int pageIndex, string url)
        {
            #region Contracts

            if (string.IsNullOrEmpty(url) == true) throw new ArgumentNullException();

            #endregion

            // QueryString
            var queryString = this.CreateHttpContent(pageIndex);
            if (string.IsNullOrEmpty(queryString) == true) return url;

            // Url
            if (url.EndsWith("?") == false) url += "?";
            url += queryString;

            // Return
            return url;
        }

        public string CreateHttpContent(int pageIndex)
        {
            // Result
            List<string> httpContentStringList = new List<string>();

            // ActionModelString
            if (this.ActionModel != null)
            {
                var actionModelString = ModelSerializer.Serialize(this.ActionModel, this.ActionModelName);
                if (string.IsNullOrEmpty(actionModelString) == false) httpContentStringList.Add(actionModelString);
            }

            // PageString
            var pageStringDictionary = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(this.PageIndexName) == false) pageStringDictionary.Add(this.PageIndexName, pageIndex.ToString());
            if (string.IsNullOrEmpty(this.ItemCountName) == false) pageStringDictionary.Add(this.ItemCountName, this.ItemCount.ToString());
            if (string.IsNullOrEmpty(this.PageSizeName) == false) pageStringDictionary.Add(this.PageSizeName, this.PageSize.ToString());

            var pageString = string.Join("&", pageStringDictionary.Select(x => (Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value.ToString()))));
            if (string.IsNullOrEmpty(pageString) == false) httpContentStringList.Add(pageString);

            // Return
            return string.Join("&", httpContentStringList);
        }
    }
}
