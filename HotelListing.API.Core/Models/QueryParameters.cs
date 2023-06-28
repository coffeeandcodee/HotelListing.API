namespace HotelListing.API.Models
{
    public class QueryParameters
    {
        //defaulting to 15 records per page
        private int _pageSize = 15;
        public int StartIndex { get; set; }
        public int PageNumber { get; set; }

        //allowing client to change default _pageSize
        public int PageSize 
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value; 
            }
        }
    }
}

