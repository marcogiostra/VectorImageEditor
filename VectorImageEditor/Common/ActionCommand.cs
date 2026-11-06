using System;

namespace VectorImageEditor.Common
{
    /// <summary>
    /// Definizione di una azione di comando
    /// </summary>
    public class ActionCommand : UndoableCommand
    {
        readonly Action _redo;
        readonly Action _undo;

        #region Class
        public ActionCommand(Action undo, Action redo)
        {
            _redo = redo;
            _undo = undo;
        }
        #endregion Class

        #region OVERRIDE
        public override void Undo()
        {
            if (_undo != null)
                _undo();
        }

   
        public override void Redo()
        {
            if (_redo != null)
                _redo();
        }
        #endregion OVERRIDE
    }
}