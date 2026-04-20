namespace BridgeWalker.Scripts.Domain.ValueObjects
{
    public readonly struct SceneLabel
    {
        public string Value { get; }

        private SceneLabel(string value)
        {
            Value = value;
        }

        public static readonly SceneLabel Title = new  ("TitleScene");
        public static readonly SceneLabel Option = new ("OptionScene");
        public static readonly SceneLabel Credit = new ("CreditScene");
        public static readonly SceneLabel InGame = new ("InGameScene");

        public override string ToString() => Value;

        public bool Equals(SceneLabel other) => Value == other.Value;
        public override bool Equals(object obj) => obj is SceneLabel other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(SceneLabel a, SceneLabel b) => a.Equals(b);
        public static bool operator !=(SceneLabel a, SceneLabel b) => !a.Equals(b);
    }
}