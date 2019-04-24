namespace Models.CLEM.Activities
{
    class ResourceActivitySell : Node
    {
        public string AccountName { get; set; }

        public string ResourceTypeName { get; set; }

        public double AmountReserved { get; set; }

        public int OnPartialResourcesAvailableAction { get; set; }

        public ResourceActivitySell(Node parent) : base(parent)
        { }
    }
}
