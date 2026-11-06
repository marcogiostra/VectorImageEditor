using EditorModel.Figures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorModel.Renderers
{
   
    [Serializable]
    public abstract class RendererDecorator : Renderer
    {
        private readonly Renderer _renderer;

        protected RendererDecorator(Renderer renderer)
        {
            _renderer = renderer;
        }

        public static List<RendererDecorator> GetDecorators(Renderer renderer, List<RendererDecorator> list = null)
        {
            if (list == null) list = new List<RendererDecorator>();
            var rendererDecorator = renderer as RendererDecorator;
            if (rendererDecorator == null) return list;
            list.Add(rendererDecorator);
            return GetDecorators(rendererDecorator._renderer, list);
        }

       
        public static bool ContainsType(Renderer renderer, Type decoratorType)
        {
            var rendererDecorator = renderer as RendererDecorator;
            if (rendererDecorator == null) return false;
            return rendererDecorator.GetType() == decoratorType || 
                ContainsType(rendererDecorator._renderer, decoratorType);
        }

        public static Renderer GetDecorator(Renderer renderer, Type decoratorType)
        {
            var rendererDecorator = renderer as RendererDecorator;
            if (rendererDecorator == null) return null;
            if (rendererDecorator.GetType() == decoratorType) return renderer;
            return GetDecorator(rendererDecorator._renderer, decoratorType);            
        }

      
        public static Renderer GetBaseRenderer(Renderer renderer)
        {
            var rendererDecorator = renderer as RendererDecorator;
            return rendererDecorator == null ? renderer : GetBaseRenderer(rendererDecorator._renderer);
        }

        public static bool ContainsAnyDecorator(Renderer renderer)
        {
            return renderer as RendererDecorator != null;
        }

        public static bool ExistsWithoutThisDecorator(IEnumerable<Figure> figures, Type type)
        {
            return figures.Count(figure => !ContainsType(figure.Renderer, type)) > 0;
        }

        public static IEnumerable<Figure> WhereNotContainsDecorator(IEnumerable<Figure> figures, Type type)
        {
            return figures.Where(figure => !ContainsType(figure.Renderer, type));
        }

        public static IEnumerable<Figure> WhereContainsDecorator(IEnumerable<Figure> figures, Type type)
        {
            return figures.Where(figure => ContainsType(figure.Renderer, type));
        }
    }
}