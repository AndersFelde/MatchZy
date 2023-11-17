namespace Get5
{
    class StringChoiceField
    {
        private string? _Value;
        public string? Value
        {
            get { return _Value; }
            set
            {
                if (value != null && !Choices.Contains(value))
                {
                    throw new ArgumentException($"Invalid value: {value}. Value must be one of the choices.");
                }
                _Value = value;
            }
        }
        List<string> Choices { get; set; }

        public StringChoiceField(List<string> choices)
        {
            this.Choices = choices;
        }
    }
}