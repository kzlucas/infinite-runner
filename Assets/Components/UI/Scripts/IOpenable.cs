namespace Components.UI.Scripts
{
    public interface IOpenable
    {
        public bool IsOpen { get; set; }

        public void Open();
        public void Close();

    }
}