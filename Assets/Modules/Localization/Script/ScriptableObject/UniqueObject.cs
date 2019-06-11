namespace Dan.Localization
{
    public class UniqueObject
    {
        /// <summary>
        /// Unique Guid
        /// </summary>
        public string Id;

        /// <summary>
        /// Name of the custom scriptable object used for editor interface
        /// </summary>
        public string EditorName;


        public virtual string GetDropDownName()
        {
            return EditorName;
        }

        /// <summary>
        /// Generate a guid (replace the actual id)
        /// </summary>
        public void GenerateGuid()
        {
            Id = System.Guid.NewGuid().ToString();
        }
    }
}
