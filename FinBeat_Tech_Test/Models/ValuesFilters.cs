namespace FinBeat_Tech_Test.Models
{
    public enum OrderBy
    {
        asc,
        desc
    }

    public class ValuesFilters
    {
        public int? CodeFrom { get; private set; }
        public int? CodeTo { get; private set; }
        public  OrderBy? OrderById { get; private set; }
        public OrderBy? OrderByCode { get; private set; }

        public ValuesFilters(int? codeFrom = null, int? codeTo = null, OrderBy? orderById = null, OrderBy? orderByCode = null) 
        {
            CodeFrom = codeFrom;
            CodeTo = codeTo;
            OrderById = orderById;
            OrderByCode = orderByCode;
        }
    }
}
