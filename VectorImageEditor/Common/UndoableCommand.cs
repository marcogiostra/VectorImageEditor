namespace VectorImageEditor.Common
{
    /// <summary>
    /// CLASSE ABSTRACT: definizione di un comando che possa annullare
    /// </summary>
    public abstract class UndoableCommand
    {
        public string Name { get; set; }
        public abstract void Undo();
        public abstract void Redo();
    }
}