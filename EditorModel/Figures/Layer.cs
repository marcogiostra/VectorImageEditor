using System;
using System.Collections.Generic;
using EditorModel.Style;
using System.Drawing;
using System.Linq;

namespace EditorModel.Figures
{
    [Serializable]
    [Flags]
    public enum LayerAllowedOperations : uint
    {
        None = 0x0,         // 
        Visible = 0x1,      // 
        Print = 0x2,        // 
        Actived = 0x4,      // 
        Locking = 0x8,      // 
        // 

        All = 0xffffffff,   // 
    }

    /// <summary>
    /// Item di un layer
    /// </summary>
    [Serializable]
    public class LayerItem
    {
        #region DICHIARAZIONI
        private readonly List<Figure> _figures = new List<Figure>();
        public List<Figure> Figures
        {
            get { return _figures; }
            set
            {
                _figures.Clear();
                _figures.AddRange(value);
            }
        }

        public string Name { get; set; }

        public LayerAllowedOperations AllowedOperations { get; private set; }

        #endregion DICHIARAZIONI

        #region CLass
        public LayerItem()
        {
            AllowedOperations = LayerAllowedOperations.All ^ (LayerAllowedOperations.Actived | LayerAllowedOperations.Locking);
        }
        #endregion CLass


        #region OPERAZIONI_PERMESSE

        public void AddAllowedOperation(LayerAllowedOperations op)
        {
            AllowedOperations |= op; 
        }

        public void RemoveAllowedOperation(LayerAllowedOperations op)
        {
            AllowedOperations &= LayerAllowedOperations.All ^ op;
        }
        #endregion OPERAZIONI_PERMESSE


        public override string ToString()
        {
            return Name;
        }

    }

   /// <summary>
   /// Layer: livello di disegno
   /// </summary>
    [Serializable]
    public class Layer
    {
        #region DICHIARAZIONI
        private readonly List<Figure> _figures = new List<Figure>();
        private List<LayerItem> _layers = new List<LayerItem>();
        private readonly List<Figure> _groups = new List<Figure>();

        public List<Figure> Groups
        {
            get { return _groups; }
            set
            {
                _groups.Clear();
                _groups.AddRange(value);
            }
        }

        public List<Figure> Figures
        {
            get { return _figures; }
            set
            {
                _figures.Clear();
                _figures.AddRange(value);
            }
        }

        public List<LayerItem> Layers
        {
            get
            {
                return _layers;
            }
            set
            {
                if (_layers == null) _layers = new List<LayerItem>();
                _layers.Clear();
                _layers.AddRange(value);
            }
        }

       
        public Fill FillStyle { get; set; }
        #endregion DICHIARAZIONI

        #region Class
        public Layer()
        {
            FillStyle = new DefaultFill { IsVisible = false };
            _figures = new List<Figure>();
            _layers = new List<LayerItem>();
            _groups = new List<Figure>();
        }
        #endregion Class

        #region FIGURE_DI_UN_LAYER

        /// <summary>
        /// Verifica se una figura appartenga al layer
        /// </summary>
        /// <param name="fig"></param>
        /// <returns></returns>
        public bool AssignedToLayer(Figure fig)
        {
            foreach (var layer in Layers)
            {
                if (layer.Figures.Contains(fig))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica se la figura del layer sia visibile nel layer 
        /// </summary>
        /// <param name="fig"></param>
        /// <returns></returns>
        public bool IsVisible(Figure fig)
        {
            foreach (var layer in Layers.Where(layer =>  layer.AllowedOperations.HasFlag(LayerAllowedOperations.Visible)))
            {
                if (layer.Figures.Contains(fig))
                    return true;
            }

            foreach (var layer in Layers.Where(layer => !layer.AllowedOperations.HasFlag(LayerAllowedOperations.Visible)))
            {
                if (layer.Figures.Contains(fig))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Verifica se òa figura sia locked nel layer
        /// </summary>
        /// <param name="fig"></param>
        /// <returns></returns>
        public bool IsLocked(Figure fig)
        {
            foreach (var layer in Layers.Where(layer =>
                                 layer.AllowedOperations.HasFlag(LayerAllowedOperations.Locking)))
            {
                if (layer.Figures.Contains(fig))
                    return true;
            }
            return false;
        }
        #endregion FIGURE_DI_UN_LAYER
    }
}
