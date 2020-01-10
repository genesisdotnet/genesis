namespace Genesis
{
    public class RelationshipGraph
    {
        public string ForeignTable { get; set; } = "";
        public string ForeignColumn { get; set; } = "";
        public string PrimaryTable { get; set; } = "";
        public string PrimaryColumn { get; set; } = "";
    }
}
