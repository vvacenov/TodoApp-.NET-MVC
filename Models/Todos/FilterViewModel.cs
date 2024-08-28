namespace TodoApp.Models.Todos
{
    public class FilterViewModel
    {
        
        public FilterViewModel(string filter)
        {
            TodoFilter = filter ?? DefaultFilter;
            string[] filters = TodoFilter.Split('-');
            Due = filters[0];
            StatusId = filters[1];
        }

        public string TodoFilter { get; }
        public string Due { get;}
        public string StatusId { get;}
        public bool HasDue => Due.ToLower() != "all";
        public bool HasStatus => StatusId.ToLower() != "all";
        public bool isUpcoming => Due.ToLower() == "upcoming";
        public bool isToday => Due.ToLower() == "today";
        public bool isPrevious => Due.ToLower() == "previous";

             public static Dictionary<string, string> DueFilterValues =>
           new Dictionary<string, string> {

               { "today", "Today" },
               { "upcoming", "Upcoming" },
               { "previous", "Previous" }
           };

        private readonly string DefaultFilter = "all-all";


    }
}
