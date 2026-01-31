namespace Components.UI.Scripts
{
    public interface IOpenable
    {
        public bool isOpen { get; set; }

        public void Open();
        public void Close();

    }
}