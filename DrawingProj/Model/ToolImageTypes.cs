namespace DrawingProj.Model
{
    /// <summary>
    /// List of all image keys for a tool
    /// </summary>
    internal class ToolImageTypes
    {
        #region Public Fields

        /// <summary>
        /// The key for the image responsible for part
        /// of the tool that should be painted in the selected color
        /// </summary>
        public static readonly string ToolColorImageKey = "Color";

        /// <summary>
        /// The key for the image responsible for shape image
        /// that the tool will draw from.
        /// </summary>
        public static readonly string ToolFormsImageKey = "Form";

        /// <summary>
        /// The key for the image responsible for active mode
        /// </summary>
        public static readonly string ToolMainImageKey = "Main";

        /// <summary>
        /// The key for the image responsible for not active mode
        /// </summary>
        public static readonly string ToolNotActiveImageKey = "NotActive";

        #endregion Public Fields
    }
}