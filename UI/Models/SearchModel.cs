namespace UI.Models
{
    public record SearchModel
    {
        public SearchModel()
        {

        }

        public SearchModel(SearchModel model)
        {
            From = model.From;
            Name = model.Name;
            Postcode = model.Postcode;
            Subjects = model.Subjects;
            TuitionType = model.TuitionType;
            KeyStages = model.KeyStages;
        }

        public ReferrerList? From { get; set; }

        public string? Name { get; set; }

        public string? Postcode { get; set; }

        public string[]? Subjects { get; set; }

        public TuitionType? TuitionType { get; set; }

        public KeyStage[]? KeyStages { get; set; }
    }
}
