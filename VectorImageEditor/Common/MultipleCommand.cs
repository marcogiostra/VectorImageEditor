namespace VectorImageEditor.Common
{
    /// <summary>
    /// CLASSE CHE CONSENTE DI ESEGUIRE PIÙ COMANDI INSIEME, PERMETTENDO DI ANNULLARE E RIPRISTINARE TUTTI I COMANDI IN UN UNICO PASSAGGIO.
    /// </summary>
    public class MultipleCommand : UndoableCommand
    {
        readonly UndoableCommand[] _commands;

        public MultipleCommand(params UndoableCommand[] commands)
        {
            _commands = commands;
        }

        public override void Undo()
        {
            for(var i = _commands.Length - 1; i>=0; i--)
                _commands[i].Undo();
        }

        public override void Redo()
        {
            foreach (var cmd in _commands)
                cmd.Redo();
        }
    }
}