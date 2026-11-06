using System;
using EditorModel.Common;
using EditorModel.Figures;
using VectorImageEditor.Common;

namespace VectorImageEditor.Controllers
{
    /// <summary>
    /// Gestione delle operazioni di annullamento/ripristino
    /// </summary>
    public class UndoRedoController
    {
        private Layer _snapshot;
        private readonly Layer _layer;
        private string _operationName;

        public UndoRedoController(Layer layer)
        {
            _layer = layer;
        }

        public void OnStartOperation(string operationName)
        {
            _snapshot = _layer.DeepClone();
            _operationName = operationName;
        }

        public void OnFinishOperation()
        {
            var afterOperationSnapshot = _layer.DeepClone();
            var beforeOperationSnapshot = _snapshot; 

            Action undo = () =>
                {
                    _layer.Figures = beforeOperationSnapshot.DeepClone().Figures;
                    _layer.FillStyle = beforeOperationSnapshot.DeepClone().FillStyle;
                    _layer.Layers = beforeOperationSnapshot.DeepClone().Layers;
                };

            Action redo = () =>
                {
                    _layer.Figures = afterOperationSnapshot.DeepClone().Figures;
                    _layer.FillStyle = afterOperationSnapshot.DeepClone().FillStyle;
                    _layer.Layers = afterOperationSnapshot.DeepClone().Layers;
                };

            UndoRedoManager.Instance.Add(new ActionCommand(undo, redo) {Name = _operationName});
        }
    }
}
