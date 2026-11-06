using EditorModel.Common;
using EditorModel.Figures;
using EditorModel.Geometry;
using EditorModel.Renderers;
using EditorModel.Selections;
using EditorModel.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using Svg;
using VectorImageEditor.Common;
using VectorImageEditor.Controllers;
using VectorImageEditor.Controls;
using EditorModel.Figures.Extentions;

namespace VectorImageEditor.DevWithGiosh
{
    
    #region ENUM
    public enum SvgGenerationTarget
    {
        ApplicationMemory,
        WindowsClipboard
    }

    public enum SvgImportSource
    {
        ApplicationMemory,
        WindowsClipboard
    }


    public enum MacroGeometrySelectionMode
    {
        None = 0,
        Arrow = 1,
        Polyline = 2,
        Bezier = 3,
        Rectangle = 4,
        Rounded = 5,
        Square = 6,
        Ellipse = 7,
        Circle = 8,
        Arc = 9,
        Chord = 10,
        Pie = 11,
        Romb = 12,
        TextBlock = 13,
        Text = 14,
        Textonbezier = 15,
        Image = 16,
        Regular3 = 17,
        Regular4 = 18,
        Regular5 = 19,
        Regular6 = 20,
        Regular7 = 21,
        Regular8 = 22,
        Regular9 = 23,
        Regular10 = 24,
        Svg = 25,
        PalleggioBasket = 26,
        PassaggioBasket = 27,
        SpostamentoBasket = 28,
        BloccoBasket = 29,
        TiroBasket = 30,
        SpostamentoBasketBezier = 31,
        BloccoBasketBezier = 32,
        PalleggioBasketBezier = 33,
        PassaggioBasketBezier = 34,
        TiroBasketBezier = 35,
        GiocatoreAttaccoRotondo = 36,
        GiocatoreDifesaTriangolare = 37,
        GiocatoreStilizzato = 38,
        GiocatoreStilizzatoSemplice = 39,
        Giocatore = 40,
        OggettoBasket = 41,
        BallBasket = 42,
        GiocatoreX = 43,
        GiocatoreNumero = 44,
        GiocatoreO = 45,

        SemicerchioSopra = 50,
        SemicerchioSotto = 51,
        

        FibaOffense = 201,

        //basket
        OggettoCampoBasketAttacco = 501,
        OggettoCampoBasketDifesa = 502,

    }

    public enum MacroGeometrySelectionSwitchMode
    {
        None = 0,
        Cyrcle = 1,
        Ellipse = 2,
        Rectangle = 3,
        Square = 4,
        Romb = 5,
        Regular5gon = 6,
        Regular6gon = 7,
        Regular7gon = 8,
        Regular8gon = 9,
        Regular9gon = 10,
        Regular10gon = 11,
        RegularTriangle = 12,
    }

    public enum MacroFillSelection
    {
        SolidBrush = 1,
        LinearGradientBrush = 2,
        RadialGradientBrush = 3,
        HatchBrush = 4,
        TextureBrush = 5
    }

    public enum MacroEffectSelection
    {
        None = 0,
        Shadow = 1,
        Glow = 2,
        TextBlock = 3,
        Warp = 4,
        Arrows = 5
    }

    #endregion ENUM

    public class MacroActions
    {
        #region DICHIARAZIONI

        #region PARAMETERI
        private Panel panelForScroll;
        private PictureBox pbCanvas;
        #endregion PARAMETRI

        #region PROPERTY

        #region Oggetti di lavoro
        public Layer _layer;
        public SelectionController _selectionController;
        public UndoRedoController _undoRedoController;
        #endregion Oggetti di lavoro

        private MacroGeometrySelectionMode _geometryModeSelected = MacroGeometrySelectionMode.None;
        public MacroGeometrySelectionMode GeometryModeSelected 
        { 
            get { return _geometryModeSelected; }
            set 
            { 
                _geometryModeSelected = value;
                DWG_GeometrySelected.GeometrySelected = value;
            }
        } 
        public MacroGeometrySelectionSwitchMode GeometrySwitchSelected { get;  set; } = MacroGeometrySelectionSwitchMode.None;
        public MacroFillSelection FillSelected { get; set; } = MacroFillSelection.SolidBrush;
        public MacroEffectSelection EffectSelected { get; set; } = MacroEffectSelection.None;

        public CourtyCampionato Campionato = CourtyCampionato.NONE;
        public BasketCourtObjectType OggettoCampoBasket = BasketCourtObjectType.None;
        public bool DrawFullCamp = false;
        public bool DrawPalazzetto = false;
        #endregion PROPERTY

        #region EVENTS
        public event EventHandler<ControlStateChangedEventArgs> ControlEnabledChanged;
        public event EventHandler EventUpdateFiguresTree;
        public event EventHandler EventUpdateInterface;
        public event EventHandler EventBuildInterface;
        public event EventHandler<string> EventGeometySwitcherd;
        public event EventHandler<string> EventInfoSelection;
        public event EventHandler<string> EventFillSelection;
        public event EventHandler<string> EventEffectSelection;
        #endregion EVENTS

        private string _fileName = string.Empty;
        public bool FileChanged { get; set; } = false;
        public string GeneratedSvgInMemory { get; private set; }
        #endregion DICHIARAZIONI

        #region CLASS

        public MacroActions()
        {
            _layer = new Layer();
        }
        public MacroActions(Panel PanelForScroll, PictureBox Canvas)
        {
            panelForScroll = PanelForScroll;
            pbCanvas = Canvas;
            _layer = new Layer();
            ConnectMethods();
        }


        #endregion CLASS

        #region GEOMETRY
        public void SetGeometrySelected(string pValue)
        {
            pValue = Regex.Replace(pValue, @"\s+", "");
            _selectionController.Clear();
            GeometryModeSelected = (MacroGeometrySelectionMode)Enum.Parse(typeof(MacroGeometrySelectionMode), pValue);
        }
        #endregion GEOMETRY

        #region GEOMETRY_SWITCH
        public MacroGeometrySelectionSwitchMode GetGemotrySwitchSelected(string nomeForma)
        {
            if (Enum.TryParse(nomeForma, true, out MacroGeometrySelectionSwitchMode risultato))
            {
                GeometrySwitchSelected = risultato;
                return risultato;
            }

            // Se la stringa non è valida, restituisce il valore di default (None)
            GeometrySwitchSelected = MacroGeometrySelectionSwitchMode.None;
            return MacroGeometrySelectionSwitchMode.None;
        }
        public void ChangePrimitiveGeometry(object sender, MacroGeometrySelectionSwitchMode pValue, String pTAG)
        {
            if (pValue == MacroGeometrySelectionSwitchMode.None) return;

            //aggiorna il testo del pulsante di selezione della geometria di switch
            string testo = @"Geometry: " + sender.ToString();
            EventGeometySwitcherd?.Invoke(null, testo);

            var primitives = _selectionController.Selection.Where(f => f.Geometry is PrimitiveGeometry && f.Renderer is DefaultRenderer).ToList();

            if (!string.IsNullOrEmpty(pTAG))
            {
                var vertexCount = int.Parse(pTAG);
                OnLayerStartChanging("Change to Regular Geometry");
                foreach (var figure in primitives)
                    FigureBuilder.BuildRegularGeometry(figure, vertexCount);
                OnLayerChanged();
            }
            else if (pValue == MacroGeometrySelectionSwitchMode.Cyrcle)
            {
                OnLayerStartChanging("Change to Cyrcle Geometry");
                foreach (var figure in primitives)
                    FigureBuilder.BuildCircleGeometry(figure);
                OnLayerChanged();
            }
            else if (pValue == MacroGeometrySelectionSwitchMode.Ellipse)
            {
                OnLayerStartChanging("Change to Ellipse Geometry");
                foreach (var figure in primitives)
                    FigureBuilder.BuildEllipseGeometry(figure);
                OnLayerChanged();
            }
            else if (pValue == MacroGeometrySelectionSwitchMode.Rectangle)
            {
                OnLayerStartChanging("Change to Rectangle Geometry");
                foreach (var figure in primitives)
                    FigureBuilder.BuildRectangleGeometry(figure);
                OnLayerChanged();
            }
            else if (pValue == MacroGeometrySelectionSwitchMode.Square)
            {
                OnLayerStartChanging("Change to Square Geometry");
                foreach (var figure in primitives)
                    FigureBuilder.BuildSquareGeometry(figure);
                OnLayerChanged();
            }

            _selectionController.UpdateMarkers();
            BuildInterface();
            EventUpdateFiguresTree?.Invoke(null, EventArgs.Empty);



        }
        #endregion GEOMETRY_SWITCH

        #region FILL
        public MacroFillSelection GetFillSelected(string nomeFill)
        {
        if (Enum.TryParse(nomeFill, true, out MacroFillSelection risultato))
        {
                FillSelected = risultato;
                return risultato;
        }

            // Se la stringa non è valida, restituisce il valore di default (None)
            FillSelected = MacroFillSelection.SolidBrush;
            return MacroFillSelection.SolidBrush;
        }

        public void ChangeFillBrush(object sender, MacroFillSelection pValue)
        {
            var exists = GetSelectionControllerSelection.Count > 0;
            if (!exists) return;

            EventFillSelection?.Invoke (null, @"Fill: " + sender.ToString());
            
            var figures = GetSelectionControllerSelection.ToList();
            if (pValue == MacroFillSelection.SolidBrush)
            {
                var decoratorsExists = figures.Count(figure =>
                    FillDecorator.ContainsAnyDecorator(figure.Style.FillStyle)) > 0;
                if (decoratorsExists)
                {
                    OnLayerStartChanging("Reset Figure Fill");
                    foreach (var figure in figures)
                    {
                        if (figure.Style.FillStyle == null) continue;
                        figure.Style.FillStyle = FillDecorator.GetBaseFill(figure.Style.FillStyle);
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroFillSelection.LinearGradientBrush)
            {
                if (FillDecorator.ExistsWithoutThisDecorator(figures, typeof(LinearGradientFill)))
                {
                    OnLayerStartChanging("Change Line Gradient Fill Brush");
                    foreach (var figure in FillDecorator.WhereContainsDecorator(figures, typeof(LinearGradientFill)))
                    {
                        if (figure.Style.FillStyle == null) continue;
                        if (figure.Style.FillStyle.AllowedDecorators.HasFlag(AllowedFillDecorators.LinearGradient))
                            figure.Style.FillStyle = new LinearGradientFill(figure.Style.FillStyle);
                        else
                        {
                            var baseFill = FillDecorator.GetBaseFill(figure.Style.FillStyle);
                            if (baseFill.AllowedDecorators.HasFlag(AllowedFillDecorators.LinearGradient))
                                figure.Style.FillStyle = new LinearGradientFill(FillDecorator.GetBaseFill(figure.Style.FillStyle));
                        }
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroFillSelection.RadialGradientBrush)
            {
                if (FillDecorator.ExistsWithoutThisDecorator(figures, typeof(RadialGradientFill)))
                {
                    OnLayerStartChanging("Change Radial Gradient Fill Brush");
                    foreach (var figure in FillDecorator.WhereContainsDecorator(figures, typeof(RadialGradientFill)))
                    {
                        if (figure.Style.FillStyle == null) continue;
                        if (figure.Style.FillStyle.AllowedDecorators.HasFlag(AllowedFillDecorators.RadialGradient))
                            figure.Style.FillStyle = new RadialGradientFill(figure.Style.FillStyle);
                        else
                        {
                            var baseFill = FillDecorator.GetBaseFill(figure.Style.FillStyle);
                            if (baseFill.AllowedDecorators.HasFlag(AllowedFillDecorators.RadialGradient))
                                figure.Style.FillStyle = new RadialGradientFill(FillDecorator.GetBaseFill(figure.Style.FillStyle));
                        }
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroFillSelection.HatchBrush)
            {
                if (FillDecorator.ExistsWithoutThisDecorator(figures, typeof(HatchFill)))
                {
                    OnLayerStartChanging("Change Hatch Fill Brush");
                    foreach (var figure in FillDecorator.WhereContainsDecorator(figures, typeof(HatchFill)))
                    {
                        if (figure.Style.FillStyle == null) continue;
                        if (figure.Style.FillStyle.AllowedDecorators.HasFlag(AllowedFillDecorators.Hatch))
                            figure.Style.FillStyle = new HatchFill(figure.Style.FillStyle);
                        else
                        {
                            var baseFill = FillDecorator.GetBaseFill(figure.Style.FillStyle);
                            if (baseFill.AllowedDecorators.HasFlag(AllowedFillDecorators.Hatch))
                                figure.Style.FillStyle = new HatchFill(FillDecorator.GetBaseFill(figure.Style.FillStyle));
                        }
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroFillSelection.TextureBrush)
            {
                if (FillDecorator.ExistsWithoutThisDecorator(figures, typeof(TextureFill)))
                {
                    OnLayerStartChanging("Change Texture Fill Brush");
                    foreach (var figure in FillDecorator.WhereContainsDecorator(figures, typeof(TextureFill)))
                    {
                        if (figure.Style.FillStyle == null) continue;
                        if (figure.Style.FillStyle.AllowedDecorators.HasFlag(AllowedFillDecorators.Texture))
                            figure.Style.FillStyle = new TextureFill(figure.Style.FillStyle);
                        else
                        {
                            var baseFill = FillDecorator.GetBaseFill(figure.Style.FillStyle);
                            if (baseFill.AllowedDecorators.HasFlag(AllowedFillDecorators.Texture))
                                figure.Style.FillStyle = new TextureFill(FillDecorator.GetBaseFill(figure.Style.FillStyle));
                        }
                    }
                    OnLayerChanged();
                }
            }
            _selectionController.UpdateMarkers();
            BuildInterface();
        }
        #endregion FILL

        #region EFFECT
        public MacroEffectSelection GetEffectSelected(string nomeEffect)
        {
            if (Enum.TryParse(nomeEffect, true, out MacroEffectSelection risultato))
            {
                EffectSelected = risultato;
                return risultato;
            }

            // Se la stringa non è valida, restituisce il valore di default (None)
            EffectSelected = MacroEffectSelection.None;
            return MacroEffectSelection.None;
        }
        public void ChangeEffects(object sender, MacroEffectSelection pValue)
        {
            var exists = GetSelectionControllerSelection.Count > 0;
            if (!exists) return;

            EventEffectSelection?.Invoke (null, @"Effect: " + sender.ToString());
            
            var figures = GetSelectionControllerSelection.ToList();
            if (pValue == MacroEffectSelection.None)
            {
                var decoratorsExists = figures.Count(figure =>
                    RendererDecorator.ContainsAnyDecorator(figure.Renderer)) > 0;
                if (decoratorsExists)
                {
                    OnLayerStartChanging("Reset Figure Effect");
                    foreach (var figure in figures)
                        figure.Renderer = RendererDecorator.GetBaseRenderer(figure.Renderer);
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroEffectSelection.Shadow)
            {
                if (RendererDecorator.ExistsWithoutThisDecorator(figures, typeof(ShadowRendererDecorator)))
                {
                    OnLayerStartChanging("Shadow Figure Effect");
                    foreach (var figure in RendererDecorator.WhereNotContainsDecorator(figures, typeof(ShadowRendererDecorator)))
                    {
                        if (figure.Renderer.AllowedDecorators.HasFlag(AllowedRendererDecorators.Shadow))
                            figure.Renderer = new ShadowRendererDecorator(figure.Renderer);
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroEffectSelection.Glow)
            {
                if (RendererDecorator.ExistsWithoutThisDecorator(figures, typeof(GlowRendererDecorator)))
                {
                    OnLayerStartChanging("Glow Figure Effect");
                    foreach (var figure in RendererDecorator.WhereNotContainsDecorator(figures, typeof(GlowRendererDecorator)))
                    {
                        if (figure.Renderer.AllowedDecorators.HasFlag(AllowedRendererDecorators.Glow))
                            figure.Renderer = new GlowRendererDecorator(figure.Renderer);
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroEffectSelection.TextBlock)
            {
                if (!figures.Any(f => f.Geometry is TextGeometry) &&
                    RendererDecorator.ExistsWithoutThisDecorator(figures, typeof(TextBlockDecorator)))
                {
                    OnLayerStartChanging("Block Text Figure Effect");
                    foreach (var figure in RendererDecorator.WhereNotContainsDecorator(figures, typeof(TextBlockDecorator)))
                    {
                        if (figure.Renderer.AllowedDecorators.HasFlag(AllowedRendererDecorators.TextBlock))
                            figure.Renderer = new TextBlockDecorator(figure.Renderer, "Text Block", new Padding(10));
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroEffectSelection.Warp)
            {
                if (RendererDecorator.ExistsWithoutThisDecorator(figures, typeof(WarpRendererDecorator)))
                {
                    OnLayerStartChanging("Warp Figure Effect");
                    foreach (var figure in RendererDecorator.WhereNotContainsDecorator(figures, typeof(WarpRendererDecorator)))
                    {
                        if (figure.Renderer.AllowedDecorators.HasFlag(AllowedRendererDecorators.Warp))
                            figure.Renderer = new WarpRendererDecorator(figure.Renderer);
                    }
                    OnLayerChanged();
                }
            }
            else if (pValue == MacroEffectSelection.Arrows)
            {
                if (RendererDecorator.ExistsWithoutThisDecorator(figures, typeof(ArrowRendererDecorator)))
                {
                    OnLayerStartChanging("Arrow Line Effect");
                    foreach (var figure in RendererDecorator.WhereNotContainsDecorator(figures, typeof(ArrowRendererDecorator)))
                    {
                        if (figure.Renderer.AllowedDecorators.HasFlag(AllowedRendererDecorators.Arrow))
                            figure.Renderer = new ArrowRendererDecorator(figure.Renderer);
                    }
                    OnLayerChanged();
                }
            }
            _selectionController.UpdateMarkers();
            BuildInterface();
        }

   

        #endregion EFFECT

        #region CONVAS
        public void Canvas_Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var scaleFactor = _selectionController.ScaleFactor;
            graphics.ScaleTransform(scaleFactor, scaleFactor);

            if (_layer.FillStyle.IsVisible)
                using (var brush = _layer.FillStyle.GetBrush(null))
                    graphics.FillRectangle(brush, pbCanvas.ClientRectangle);

            int I = -1;
            foreach (var fig in _layer.Figures.Where(fig => _layer.IsVisible(fig)))
            {
               
                fig.Renderer.Render(graphics, fig);
            }

            _selectionController.Selection.Renderer.Render(graphics, _selectionController.Selection);

            foreach (var marker in _selectionController.Markers)
            {
                marker.Transform.Matrix.Scale(1 / scaleFactor, 1 / scaleFactor);
                marker.Renderer.Render(graphics, marker);
                marker.Transform.Matrix.Scale(scaleFactor, scaleFactor);
            }
        }
        public void Canvas_MouseDown(MouseEventArgs e, Keys ModifierKeys)
        {
            if (e.Button == MouseButtons.Right && IsPolylineDrawingMode(GeometryModeSelected))
            {
                if (_selectionController.Selection.Geometry is AddLineGeometry)
                {
                    _selectionController.OnDblClick(e.Location, ModifierKeys, true);
                    GeometryModeSelected = MacroGeometrySelectionMode.None;
                }
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            if (e.Clicks == 2)
            {
                var isPoly = IsPolylineDrawingMode(GeometryModeSelected);
                _selectionController.OnDblClick(e.Location, ModifierKeys);
                if (isPoly)
                    GeometryModeSelected = MacroGeometrySelectionMode.None;

            }
            else
                _selectionController.OnMouseDown(e.Location, ModifierKeys);
        }
        public void Canvas_MouseMove(MouseEventArgs e, Keys ModifierKeys)
        {
            Cursor.Current = _selectionController.GetCursor(e.Location, ModifierKeys, e.Button);
            _selectionController.OnMouseMove(e.Location, ModifierKeys);
        }
        public void Canvas_MouseUp(MouseEventArgs e, Keys ModifierKeys)
        {
            if (e.Button == MouseButtons.Left)
                _selectionController.OnMouseUp(e.Location, ModifierKeys);

        }
        public void UpdateCanvasSize()
        {
            if (_layer == null) return;
            var size = new Size();
            foreach (var figrect in _layer.Figures.Select(figure =>
                figure.GetTransformedPath().Path.GetBounds()))
            {
                if (size.Width < figrect.Right) size.Width = (int)figrect.Right;
                if (size.Height < figrect.Bottom) size.Height = (int)figrect.Bottom;
            }

            size.Width += 50;
            size.Height += 50;

            var sf = _selectionController.ScaleFactor;
            size = new Size((int)(size.Width * sf), (int)(size.Height * sf));

            var pnlSize = panelForScroll.ClientSize;
            pbCanvas.Size = new Size(size.Width < pnlSize.Width ? pnlSize.Width : size.Width,
                                     size.Height < pnlSize.Height ? pnlSize.Height : size.Height);
        }
        public void ConvasZoneForScroll_DragDrop(string[] files, DragEventArgs e)
        {
            var figure = new Figure();
            var image = (Bitmap)System.Drawing.Image.FromFile(files[0]);
            FigureBuilder.BuildImageRenderGeometry(figure, image);
            var point = panelForScroll.PointToClient(new Point(e.X, e.Y));
            var width = image.Width;
            var height = image.Height;
            var m = new Matrix();
            m.Translate(point.X + width / 2f, point.Y + height / 2f);
            m.Scale(width, height);
            figure.Transform.Matrix = m;
            OnLayerStartChanging("Insert Image By File Drag&Drop");
            _layer.Figures.Add(figure);
            OnLayerChanged();
            _selectionController.Clear();
            GetSelectionControllerSelection.Add(figure);
            _selectionController.UpdateMarkers();
            UpdateInterface();
        }

        /// <summary>Importa l'intero documento SVG come una singola figura nel layer corrente.</summary>
        public void ImportSvg(FormVectorImageEditor frm)
        {
            using (var dialog = new OpenFileDialog
            {
                DefaultExt = "svg",
                Filter = "Immagine SVG (*.svg)|*.svg",
                Title = "Importa SVG"
            })
            {
                if (dialog.ShowDialog(frm) != DialogResult.OK) return;

                try
                {
                    var svg = SvgDocument.Open(dialog.FileName);
                    var viewBox = svg.ViewBox;
                    var vbWidth = (float)viewBox.Width;
                    var vbHeight = (float)viewBox.Height;
                    if (vbWidth <= 0 || vbHeight <= 0)
                        throw new InvalidDataException("Il file SVG non contiene un viewBox valido.");

                    var scaleFactor = Math.Max(0.01f, _selectionController.ScaleFactor);
                    // Mantiene le dimensioni intrinseche SVG espresse dal viewBox.
                    var width = vbWidth / scaleFactor;
                    var height = vbHeight / scaleFactor;
                    var centerX = (-panelForScroll.AutoScrollPosition.X + panelForScroll.ClientSize.Width / 2f) / scaleFactor;
                    var centerY = (-panelForScroll.AutoScrollPosition.Y + panelForScroll.ClientSize.Height / 2f) / scaleFactor;

                    var figure = new Figure();
                    FigureBuilder.BuildSvgRenderGeometry(figure, svg);
                    var transform = new Matrix();
                    transform.Translate(centerX, centerY);
                    transform.Scale(width, height);
                    figure.Transform.Matrix.Dispose();
                    figure.Transform.Matrix = transform;

                    OnLayerStartChanging("Import SVG");
                    _layer.Figures.Add(figure);
                    OnLayerChanged();
                    _selectionController.Clear();
                    GetSelectionControllerSelection.Add(figure);
                    _selectionController.UpdateMarkers();
                    UpdateInterface();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(frm, "Impossibile importare il file SVG." + Environment.NewLine + ex.Message,
                        "Importa SVG", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static bool IsPolylineDrawingMode(MacroGeometrySelectionMode mode)
        {
            return mode == MacroGeometrySelectionMode.Polyline ||
                   mode == MacroGeometrySelectionMode.Bezier ||
                   mode == MacroGeometrySelectionMode.PalleggioBasket ||
                   mode == MacroGeometrySelectionMode.PassaggioBasket ||
                   mode == MacroGeometrySelectionMode.SpostamentoBasket ||
                   mode == MacroGeometrySelectionMode.BloccoBasket ||
                   mode == MacroGeometrySelectionMode.TiroBasket ||
                   mode == MacroGeometrySelectionMode.SpostamentoBasketBezier ||
                   mode == MacroGeometrySelectionMode.BloccoBasketBezier ||
                   mode == MacroGeometrySelectionMode.PalleggioBasketBezier ||
                   mode == MacroGeometrySelectionMode.PassaggioBasketBezier ||
                   mode == MacroGeometrySelectionMode.TiroBasketBezier;
        }

        /// <summary>Importa i componenti SVG di primo livello come figure modificabili separatamente.</summary>
        public void ImportSvgFigures(FormVectorImageEditor frm)
        {
            using (var dialog = new OpenFileDialog
            {
                DefaultExt = "svg",
                Filter = "Immagine SVG (*.svg)|*.svg",
                Title = "Importa SVG come figure"
            })
            {
                if (dialog.ShowDialog(frm) != DialogResult.OK) return;
                ImportSvgFiguresFromFile(frm, dialog.FileName);
            }
        }

        /// <summary>Seleziona e importa un SVG dalla cartella Resources\SvgFigures dell'applicazione.</summary>
        /// <summary>
        /// Imports selectable SVG components into this MacroActions instance's canvas.
        /// Use the MacroActions instance constructed for the secondary canvas to target it.
        /// </summary>
        public void ImportSvgResourceFigures(Form owner)
        {
            var resourceDirectory = FindSvgResourceDirectory();
            if (string.IsNullOrEmpty(resourceDirectory) || !Directory.Exists(resourceDirectory))
            {
                MessageBox.Show(owner,
                    "La cartella delle risorse SVG non esiste. Crea la cartella Resources\\SvgFigures e inserisci al suo interno i file SVG.",
                    "Risorse SVG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Directory.GetFiles(resourceDirectory, "*.svg", SearchOption.TopDirectoryOnly).Any())
            {
                MessageBox.Show(owner,
                    "Non ci sono file SVG nella cartella Resources\\SvgFigures. Aggiungi i file SVG e riprova.",
                    "Risorse SVG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new OpenFileDialog
            {
                DefaultExt = "svg",
                Filter = "Immagine SVG (*.svg)|*.svg",
                InitialDirectory = resourceDirectory,
                Title = "Scegli un SVG dalle risorse"
            })
            {
                if (dialog.ShowDialog(owner) != DialogResult.OK) return;
                ImportSvgFiguresFromFile(owner, dialog.FileName);
            }
        }

        private static string FindSvgResourceDirectory()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(appDirectory, "Resources", "SvgFigures"),
                Path.GetFullPath(Path.Combine(appDirectory, "..", "..", "Resources", "SvgFigures"))
            };
            return candidates.FirstOrDefault(Directory.Exists) ?? candidates[0];
        }

        private void ImportSvgFiguresFromFile(Form frm, string fileName)
        {

                try
                {
                    var svg = SvgDocument.Open(fileName);
                    var viewBox = svg.ViewBox;
                    var vbWidth = (float)viewBox.Width;
                    var vbHeight = (float)viewBox.Height;
                    if (vbWidth <= 0 || vbHeight <= 0)
                        throw new InvalidDataException("Il file SVG non contiene un viewBox valido.");

                    XDocument sourceXml;
                    var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
                    using (var reader = XmlReader.Create(fileName, settings))
                        sourceXml = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
                    var root = sourceXml.Root;
                    if (root == null) throw new InvalidDataException("Il file SVG non contiene un elemento radice.");

                    var xmlComponents = root.Elements()
                        .Where(element => !IsSvgNonDrawingElement(element.Name.LocalName))
                        .ToList();
                    // Bounds è esposto da SvgVisualElement, non dalla classe base SvgElement.
                    var svgComponents = svg.Children.OfType<SvgVisualElement>()
                        .Where(element => !IsSvgNonDrawingElement(GetSvgElementLocalName(element)))
                        .ToList();
                    if (xmlComponents.Count != svgComponents.Count)
                        throw new InvalidDataException("Non è stato possibile associare i componenti SVG alle figure.");

                    var scaleFactor = Math.Max(0.01f, _selectionController.ScaleFactor);
                    var figures = new List<Figure>();

                    for (var i = 0; i < svgComponents.Count; i++)
                    {
                        var bounds = svgComponents[i].Bounds;
                        if (float.IsNaN(bounds.X) || float.IsNaN(bounds.Y) ||
                            float.IsInfinity(bounds.X) || float.IsInfinity(bounds.Y) ||
                            bounds.Width <= 0 || bounds.Height <= 0)
                            continue;

                        var componentRoot = new XElement(root);
                        var childElements = componentRoot.Elements().ToList();
                        var componentIndex = 0;
                        foreach (var child in childElements)
                        {
                            if (IsSvgNonDrawingElement(child.Name.LocalName)) continue;
                            if (componentIndex++ != i)
                            {
                                child.Remove();
                            }
                            else
                            {
                                var translatedComponent = new XElement(
                                    child.Name.Namespace + "g",
                                    new XAttribute("transform", string.Format(CultureInfo.InvariantCulture,
                                        "translate({0} {1})", -bounds.X, -bounds.Y)),
                                    new XElement(child));
                                child.ReplaceWith(translatedComponent);
                            }
                        }

                        componentRoot.SetAttributeValue("viewBox", string.Join(" ", new[]
                        {
                            "0", "0",
                            bounds.Width.ToString("R", CultureInfo.InvariantCulture),
                            bounds.Height.ToString("R", CultureInfo.InvariantCulture)
                        }));
                        componentRoot.SetAttributeValue("width", bounds.Width.ToString("R", CultureInfo.InvariantCulture));
                        componentRoot.SetAttributeValue("height", bounds.Height.ToString("R", CultureInfo.InvariantCulture));
                        var componentSvg = SvgDocument.FromSvg<SvgDocument>(componentRoot.ToString(SaveOptions.DisableFormatting));
                        var figure = new Figure();
                        FigureBuilder.BuildSvgRenderGeometry(figure, componentSvg);

                        var width = bounds.Width / scaleFactor;
                        var height = bounds.Height / scaleFactor;
                        // Usa le coordinate originali del documento SVG senza ricentrare il gruppo.
                        var figureCenterX = (bounds.X + bounds.Width / 2f) / scaleFactor;
                        var figureCenterY = (bounds.Y + bounds.Height / 2f) / scaleFactor;
                        var transform = new Matrix();
                        transform.Translate(figureCenterX, figureCenterY);
                        transform.Scale(width, height);
                        figure.Transform.Matrix.Dispose();
                        figure.Transform.Matrix = transform;
                        figures.Add(figure);
                    }

                    if (figures.Count == 0)
                        throw new InvalidDataException("Il file SVG non contiene figure con dimensioni importabili.");

                    OnLayerStartChanging("Import SVG figures");
                    _layer.Figures.AddRange(figures);
                    OnLayerChanged();
                    _selectionController.Clear();
                    _selectionController.UpdateMarkers();
                    UpdateInterface();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(frm, "Impossibile importare le figure SVG." + Environment.NewLine + ex.Message,
                        "Importa SVG come figure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private static bool IsSvgNonDrawingElement(string localName)
        {
            return string.Equals(localName, "defs", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(localName, "style", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(localName, "title", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(localName, "desc", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(localName, "metadata", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(localName, "script", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetSvgElementLocalName(SvgElement element)
        {
            var typeName = element.GetType().Name;
            if (string.Equals(typeName, "SvgDocumentMetadata", StringComparison.Ordinal)) return "metadata";
            if (string.Equals(typeName, "SvgDefinitionList", StringComparison.Ordinal)) return "defs";
            return typeName.StartsWith("Svg", StringComparison.Ordinal) ? typeName.Substring(3) : typeName;
        }

        /// <summary>Genera in memoria un SVG composto dalle figure SVG presenti nel documento.</summary>
        public string GenerateSvgFromImportedFigures(SvgGenerationTarget target)
        {
            var svgFigures = _layer.Figures
                .Where(figure => RendererDecorator.GetBaseRenderer(figure.Renderer) is MioSvgRenderer)
                .ToList();
            if (svgFigures.Count == 0)
                throw new InvalidOperationException("Nel documento non ci sono figure SVG importate.");

            var totalBounds = RectangleF.Empty;
            foreach (var figure in svgFigures)
            {
                using (var path = figure.GetTransformedPath().Path)
                {
                    var bounds = path.GetBounds();
                    totalBounds = RectangleF.Union(totalBounds, bounds);
                }
            }
            if (totalBounds.Width <= 0 || totalBounds.Height <= 0)
                throw new InvalidOperationException("Le figure SVG non hanno dimensioni esportabili.");

            XNamespace svgNamespace = "http://www.w3.org/2000/svg";
            var root = new XElement(svgNamespace + "svg",
                new XAttribute("xmlns", svgNamespace.NamespaceName),
                new XAttribute("width", totalBounds.Width.ToString("R", CultureInfo.InvariantCulture)),
                new XAttribute("height", totalBounds.Height.ToString("R", CultureInfo.InvariantCulture)),
                new XAttribute("viewBox", string.Join(" ", new[]
                {
                    totalBounds.X.ToString("R", CultureInfo.InvariantCulture),
                    totalBounds.Y.ToString("R", CultureInfo.InvariantCulture),
                    totalBounds.Width.ToString("R", CultureInfo.InvariantCulture),
                    totalBounds.Height.ToString("R", CultureInfo.InvariantCulture)
                })));

            for (var i = 0; i < svgFigures.Count; i++)
            {
                var renderer = RendererDecorator.GetBaseRenderer(svgFigures[i].Renderer) as MioSvgRenderer;
                if (renderer == null || string.IsNullOrEmpty(renderer.SvgSource)) continue;

                var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
                XDocument source;
                using (var reader = XmlReader.Create(new StringReader(renderer.SvgSource), settings))
                    source = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
                var sourceRoot = source.Root;
                if (sourceRoot == null) continue;

                // Ogni SVG importato occupa il quadrato unitario della figura dell'editor.
                var nestedSvg = new XElement(sourceRoot);
                nestedSvg.SetAttributeValue("x", "-0.5");
                nestedSvg.SetAttributeValue("y", "-0.5");
                nestedSvg.SetAttributeValue("width", "1");
                nestedSvg.SetAttributeValue("height", "1");
                nestedSvg.SetAttributeValue("preserveAspectRatio", "none");

                var matrix = svgFigures[i].Transform.Matrix.Elements;
                var transform = string.Format(CultureInfo.InvariantCulture,
                    "matrix({0} {1} {2} {3} {4} {5})",
                    matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                root.Add(new XElement(svgNamespace + "g",
                    new XAttribute("transform", transform), nestedSvg));
            }

            var output = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), root)
                .ToString(SaveOptions.DisableFormatting);
            GeneratedSvgInMemory = output;

            if (target == SvgGenerationTarget.WindowsClipboard)
            {
                var data = new DataObject();
                data.SetData("image/svg+xml", false, output);
                data.SetText(output);
                Clipboard.SetDataObject(data, true);
            }

            return output;
        }

        /// <summary>Inserisce sulla canvas l'SVG in memoria o negli appunti come figura SVG.</summary>
        public Figure InsertGeneratedSvg(SvgImportSource source)
        {
            string svgText;
            if (source == SvgImportSource.ApplicationMemory)
            {
                svgText = GeneratedSvgInMemory;
                if (string.IsNullOrWhiteSpace(svgText))
                    throw new InvalidOperationException("Non è presente un SVG generato in memoria.");
            }
            else
            {
                svgText = null;
                if (Clipboard.ContainsData("image/svg+xml"))
                {
                    var clipboardData = Clipboard.GetData("image/svg+xml");
                    svgText = clipboardData as string;
                    if (svgText == null && clipboardData is byte[])
                        svgText = Encoding.UTF8.GetString((byte[])clipboardData);
                    if (svgText == null && clipboardData is Stream)
                    {
                        using (var reader = new StreamReader((Stream)clipboardData, Encoding.UTF8, true, 1024, true))
                            svgText = reader.ReadToEnd();
                    }
                }
                if (string.IsNullOrWhiteSpace(svgText) && Clipboard.ContainsText())
                    svgText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(svgText))
                    throw new InvalidOperationException("Gli appunti non contengono un file SVG.");
            }

            var svg = SvgDocument.FromSvg<SvgDocument>(svgText);
            var viewBox = svg.ViewBox;
            var width = (float)viewBox.Width;
            var height = (float)viewBox.Height;
            if (width <= 0 || height <= 0)
                throw new InvalidDataException("L'SVG da inserire non contiene un viewBox valido.");

            var scaleFactor = Math.Max(0.01f, _selectionController.ScaleFactor);
            var centerX = (-panelForScroll.AutoScrollPosition.X + panelForScroll.ClientSize.Width / 2f) / scaleFactor;
            var centerY = (-panelForScroll.AutoScrollPosition.Y + panelForScroll.ClientSize.Height / 2f) / scaleFactor;
            var figure = new Figure();
            FigureBuilder.BuildSvgRenderGeometry(figure, svg);

            var transform = new Matrix();
            transform.Translate(centerX, centerY);
            transform.Scale(width / scaleFactor, height / scaleFactor);
            figure.Transform.Matrix.Dispose();
            figure.Transform.Matrix = transform;

            OnLayerStartChanging("Insert SVG");
            _layer.Figures.Add(figure);
            OnLayerChanged();
            _selectionController.Clear();
            GetSelectionControllerSelection.Add(figure);
            _selectionController.UpdateMarkers();
            BuildInterface();
            return figure;
        }
        #endregion CONVAS

        #region LAYER
        public List<LayerItem> GetLayerItems
        {
            get { return _layer.Layers; }
        }

        /// <summary>
        /// Rimuove dal documento le figure appartenenti al layer indicato.
        /// Le figure assegnate anche ad altri layer e quelle senza layer restano intatte.
        /// </summary>
        public void ClearLayer(LayerItem layer)
        {
            if (layer == null || !GetLayerItems.Contains(layer) || layer.Figures.Count == 0)
                return;

            var figuresToRemove = layer.Figures.ToList();
            OnLayerStartChanging("Clear Layer");
            layer.Figures.Clear();

            foreach (var figure in figuresToRemove)
            {
                if (!GetLayerItems.Any(otherLayer => otherLayer.Figures.Contains(figure)))
                    _layer.Figures.Remove(figure);
            }

            _selectionController.Clear();
            OnLayerChanged();
            BuildInterface();
        }

        /// <summary>
        /// Duplica le figure del layer sorgente nel layer di destinazione.
        /// Le copie sono figure indipendenti e vengono aggiunte al documento.
        /// </summary>
        public void CopyLayerObjects(LayerItem sourceLayer, LayerItem destinationLayer)
        {
            if (sourceLayer == null || destinationLayer == null ||
                ReferenceEquals(sourceLayer, destinationLayer) ||
                !GetLayerItems.Contains(sourceLayer) || !GetLayerItems.Contains(destinationLayer) ||
                sourceLayer.Figures.Count == 0)
                return;

            var copies = sourceLayer.Figures.Select(figure => figure.DeepClone()).ToList();
            OnLayerStartChanging("Copy Layer Objects");

            foreach (var copy in copies)
            {
                _layer.Figures.Add(copy);
                destinationLayer.Figures.Add(copy);
            }

            _selectionController.Clear();
            OnLayerChanged();
            BuildInterface();
        }
        #endregion LAYER

        #region SelectionController

        #region PROPERTY_SelectionController
        public Selection GetSelectionControllerSelection
        {
            get { return _selectionController.Selection; }
        }



        public EditorMode SelectionControllerEditorMode
        {
            get { return _selectionController.EditorMode; }
            set { _selectionController.EditorMode = value; }
        }

       
        #endregion PROPERTY_SelectionController



        /// <summary>
        /// Prepara i gli eventi da collegare a _selectionController
        /// </summary>
        private void ConnectMethods()
        {
            _undoRedoController = new UndoRedoController(_layer);

            //inizializza il selectioncontrol
            _selectionController = new SelectionController(_layer);


            _selectionController.SelectedFigureChanged += BuildInterface;
            _selectionController.SelectedTransformChanging += UpdateInterface;
            _selectionController.SelectedTransformChanged += UpdateInterface;
            _selectionController.SelectedRangeChanging += _selectionController_SelectedRangeChanging;
            _selectionController.EditorModeChanged += _ => UpdateInterface();
            _selectionController.LayerStartChanging += () => _selectionController_LayerStartChanging();
            _selectionController.LayerChanged += OnLayerChanged;
            _selectionController.ResetFigureCreator += _selectionController_ResetCreateFigureSelector;

        }

        /// <summary>
        /// Richiama l'evento EventUpdateInterface per aggiornare l'interfaccia utente
        /// </summary>
        private void UpdateInterface()
        {
            EventUpdateInterface?.Invoke(null, EventArgs.Empty);

        }

        public void SelectionControlSelectAll()
        {
            _selectionController.SelectAllFigures();
        }
        public void SelectionAlignHorizontal(FigureAlignment alignmanet)
        {
            _selectionController.AlignHorizontal(alignmanet);
            UpdateInterface();
        }

        public void SelectionAlignVertical(FigureAlignment alignmanet)
        {
            _selectionController.AlignVertical(alignmanet);
            UpdateInterface();
        }

        public void SelectionSameResize(FigureResize figureResize)
        {
            _selectionController.SameResize(figureResize);
            UpdateInterface();
        }

        public void SelectionEvenSpaces(FigureArrange figureArrange)
        {
            _selectionController.EvenSpaces(figureArrange);
            UpdateInterface();
        }

        public void SelectionControlerScalerFactor (float pst)
        {
            
            _selectionController.Clear();
            _selectionController.ScaleFactor = pst / 100f;
            UpdateCanvasSize();
            UpdateInterface();
        }

        public void SelectionConvertToPath()
        {
            _selectionController.ConvertToPath();
        }

 

        #region EVENT_SelectionController


        private void _selectionController_SelectedRangeChanging(Rectangle rect, float angle)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Area: {0}", rect);
            if (Math.Abs(angle - 0) > float.Epsilon)
                sb.AppendFormat(" Rotate at: {0:0.0}°", angle);
            if (_selectionController.EditorMode == EditorMode.Drag &&
                _selectionController.Selection.Count > 0)
            {
                var figure = _selectionController.Selection.FirstOrDefault();
                if (figure != null)
                {
                    angle = Helper.GetAngle(figure.Transform);
                    var size = Helper.GetSize(figure.Transform);
                    sb.AppendFormat(" Figure: size {0}, rotated at {1:0.0}°", size, angle);
                }
            }

            //aggiorna il testo del messaggio di info 
            string testo = sb.ToString();
            EventInfoSelection?.Invoke(null, testo);

            UpdateInterface();
        }

        private void _selectionController_ResetCreateFigureSelector()
        {
            ControlStateChangedEventArgs ctr1 = new ControlStateChangedEventArgs("tsbArrow", false);
            ControlEnabledChanged?.Invoke(this, ctr1);
        }

        private void _selectionController_LayerStartChanging()
        {
            OnLayerStartChanging(_selectionController.EditorMode.ToString());
        }

        private void BuildInterface()
        {
            EventBuildInterface?.Invoke(null, EventArgs.Empty);
        }
        #endregion EVENT_SelectionController


        #endregion F()_SelectionController

        #region FIGURE
        public void CreateFigure(MacroGeometrySelectionMode pValue = MacroGeometrySelectionMode.None)
        {
            //pulisce 
            _selectionController.Clear();
            if (pValue == MacroGeometrySelectionMode.None) return;

            // Imposta il cursore e la funzione di creazione della figura in base al tipo selezionato
            Func<Figure> figureCreator = null;
            Cursor figureCreatorCursor = CursorFactory.GetCursor(UserCursor.SelectByRibbonRect);

            //freccia
            if (pValue == MacroGeometrySelectionMode.Arrow)
            {
                figureCreatorCursor = Cursors.Default;
                Cursor.Current = Cursors.Default;

                ControlStateChangedEventArgs ctr0 = new ControlStateChangedEventArgs("tsbArrow", false);
                ControlEnabledChanged?.Invoke(this, ctr0);

                _selectionController.CreateFigureCursor = figureCreatorCursor;
                _selectionController.CreateFigureRequest = figureCreator;
                return;
            }
            ControlStateChangedEventArgs ctr1 = new ControlStateChangedEventArgs("tsbArrow", true);
            ControlEnabledChanged?.Invoke(this, ctr1);


            //polyline
            if (pValue == MacroGeometrySelectionMode.Polyline)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreatePolyline);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildPolyGeometry(fig, false);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.PalleggioBasket)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreatePolyline);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildPalleggioBasketGeometry(fig, false);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.SpostamentoBasketBezier ||
                     pValue == MacroGeometrySelectionMode.BloccoBasketBezier ||
                     pValue == MacroGeometrySelectionMode.PalleggioBasketBezier ||
                     pValue == MacroGeometrySelectionMode.PassaggioBasketBezier ||
                     pValue == MacroGeometrySelectionMode.TiroBasketBezier)
            {
                var basketKind = pValue == MacroGeometrySelectionMode.SpostamentoBasketBezier
                    ? BasketPolylineKind.Spostamento
                    : pValue == MacroGeometrySelectionMode.BloccoBasketBezier
                        ? BasketPolylineKind.Blocco
                        : pValue == MacroGeometrySelectionMode.PalleggioBasketBezier
                            ? BasketPolylineKind.PalleggioSinusoidale
                            : pValue == MacroGeometrySelectionMode.PassaggioBasketBezier
                                ? BasketPolylineKind.PalleggioTratteggiato
                                : BasketPolylineKind.TiroPuntinato;
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreatePolyline);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildBezierGeometry(fig, new PointF[] { }, new byte[] { }, basketKind);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.PassaggioBasket ||
                     pValue == MacroGeometrySelectionMode.SpostamentoBasket ||
                     pValue == MacroGeometrySelectionMode.BloccoBasket ||
                     pValue == MacroGeometrySelectionMode.TiroBasket)
            {
                var basketKind = pValue == MacroGeometrySelectionMode.PassaggioBasket
                    ? BasketPolylineKind.PalleggioTratteggiato
                    : pValue == MacroGeometrySelectionMode.SpostamentoBasket
                        ? BasketPolylineKind.Spostamento
                        : pValue == MacroGeometrySelectionMode.BloccoBasket
                            ? BasketPolylineKind.Blocco
                            : BasketPolylineKind.TiroPuntinato;
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreatePolyline);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildBasketPolylineGeometry(fig, basketKind, false);
                    return fig;
                };
            }
          
            //bezier
            else if (pValue == MacroGeometrySelectionMode.Bezier)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreatePolyline);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildBezierGeometry(fig, new PointF[] { }, new byte[] { });
                    return fig;
                };
            }

            //RETTANGOLO
            else if (pValue == MacroGeometrySelectionMode.Rectangle)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildRectangleGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Rounded)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildRoundedRectangleGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Square)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateSquare);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildSquareGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Ellipse)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateEllipse);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildEllipseGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Circle)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildCircleGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreAttaccoRotondo)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreAttaccoRotondoGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreDifesaTriangolare)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateSquare);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreDifesaTriangolareGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreStilizzato)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateSquare);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreStilizzatoGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreStilizzatoSemplice)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreStilizzatoSempliceGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Giocatore)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreX)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreXGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreNumero)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreNumeroGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.GiocatoreO)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildGiocatoreOGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.OggettoBasket)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildOggettoBasketGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.BallBasket)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildBallBasketGeometry(fig);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Arc)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildWedgeGeometry(fig, WedgeKind.Arc);
                    return fig;
                };
            }       
            else if (pValue == MacroGeometrySelectionMode.Chord)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildWedgeGeometry(fig, WedgeKind.Chord);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Pie)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateCircle);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildWedgeGeometry(fig, WedgeKind.Pie);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Romb || pValue == MacroGeometrySelectionMode.Regular4)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildRegularGeometry(fig, 4);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.TextBlock)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateBlockText);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildTextRenderGeometry(fig, "TextBlock");
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Text)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateText);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildTextGeometry(fig, "Text");
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Textonbezier)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateBlockText);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildBezierTextGeometry(fig, "The quick brown fox jumps over the lazy dog.");
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Image)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateImage);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildImageRenderGeometry(fig, null);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Svg)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateImage);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildSvgRenderGeometry(fig, null);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.Regular3 || pValue == MacroGeometrySelectionMode.Regular4 ||
                pValue == MacroGeometrySelectionMode.Regular5 || pValue == MacroGeometrySelectionMode.Regular6 ||
                pValue == MacroGeometrySelectionMode.Regular7 || pValue == MacroGeometrySelectionMode.Regular8 ||
                pValue == MacroGeometrySelectionMode.Regular9 || pValue == MacroGeometrySelectionMode.Regular10)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                var number = (int)pValue - 14; // Regular3 is 17, Regular4 is 18, ..., Regular10 is 24
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildRegularGeometry(fig, number);
                    return fig;
                };
            }

            //parte generica nuova

            //semicerchio sopra
            else if (pValue == MacroGeometrySelectionMode.SemicerchioSopra)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildSemicerchioSopra(fig);
                    return fig;
                };
            }
            //semicerchio sopra
            else if (pValue == MacroGeometrySelectionMode.SemicerchioSotto)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildSemicerchioSotto(fig);
                    return fig;
                };
            }


            #region BASKET
            else if (pValue == MacroGeometrySelectionMode.OggettoCampoBasketAttacco)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildOggettoCampoBasketAttaccoGeometry(fig,Campionato, DrawFullCamp, DrawPalazzetto, OggettoCampoBasket);
                    return fig;
                };
            }
            else if (pValue == MacroGeometrySelectionMode.OggettoCampoBasketDifesa)
            {
                figureCreatorCursor = CursorFactory.GetCursor(UserCursor.CreateRect);
                figureCreator = () =>
                {
                    var fig = new Figure();
                    FigureBuilder.BuildOggettoCampoBasketDifesaGeometry(fig, Campionato, DrawFullCamp, DrawPalazzetto,OggettoCampoBasket);
                    return fig;
                };
            }
            #endregion BASKET




            _selectionController.CreateFigureCursor = figureCreatorCursor;
            _selectionController.CreateFigureRequest = figureCreator;
        }

        #endregion FIGURE

        #region F()_PRINCIPALI

        public void OnLayerChanged()
{
_undoRedoController.OnFinishOperation();
UpdateCanvasSize();
EventUpdateFiguresTree?.Invoke(null, EventArgs.Empty);
}
        public void StyleChanaged()
        {
        OnLayerChanged();
        _selectionController.RemoveLockedOrNotVisible();
        UpdateInterface();
        _selectionController.UpdateMarkers();
        }
        public void OnLayerStartChanging(string opName)
{
_undoRedoController.OnStartOperation(opName);
}

     

        #endregion F()_PRINCIPALI

        #region MENU_FILE
        public void CreateNew(FormVectorImageEditor frm,string pCaption)
        {
            _fileName = string.Empty;
            FileChanged = false;
            frm.Text = pCaption;
            _selectionController.Clear();
            UndoRedoManager.Instance.ClearHistory();
            _layer.FillStyle = new DefaultFill { IsVisible = false };
            _layer.Figures.Clear();
            GetLayerItems.Clear();
            BuildInterface();
        }

        public void Open (FormVectorImageEditor frm, string pCaption)
        {
            if (FileChanged)
            {
                var result = MessageBox.Show(frm, @"Il documento contiene dati non salvati." + Environment.NewLine +
                    @"Vuoi salvarli?", @"Apertura di un altro documento",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
                if (result == DialogResult.Cancel)
                    return;
                /* @@@
                if (result == DialogResult.Yes)
                    tsmSave.PerformClick();
                */
            }
            OpenFileDialog openEditorFileDialog = new OpenFileDialog();
            openEditorFileDialog.DefaultExt = "vge";
            openEditorFileDialog.Filter = "File dell'editor (*.vge)|*.vge";
            if (openEditorFileDialog.ShowDialog(frm) != DialogResult.OK) return;
            CreateNew(frm, pCaption);
            _fileName = openEditorFileDialog.FileName;
            _layer = SaverLoader.LoadFromFile(_fileName);
            ConnectMethods();
            frm.Text = pCaption + @" - " + _fileName;
            UndoRedoManager.Instance.ClearHistory();
            BuildInterface();
        }

        public void Save(FormVectorImageEditor frm, string pCaption)
        {
            if (string.IsNullOrWhiteSpace(_fileName))
            {
                SaveFileDialog saveEditorFileDialog = new SaveFileDialog();
                saveEditorFileDialog.DefaultExt = "vge";
                saveEditorFileDialog.Filter = "File dell'editor (*.vge)|*.vge";

                if (saveEditorFileDialog.ShowDialog(frm) != DialogResult.OK) return;
                _fileName = saveEditorFileDialog.FileName;
            }
            SaverLoader.SaveToFile(_fileName, _layer);
            UndoRedoManager.Instance.ClearHistory();
            frm.Text = pCaption + @" - " + _fileName;
            BuildInterface();
        }

        public void SaveAs(FormVectorImageEditor frm, string pCaption)
        {
            SaveFileDialog saveEditorFileDialog = new SaveFileDialog();
            saveEditorFileDialog.DefaultExt = "vge";
            saveEditorFileDialog.Filter = "File dell'editor (*.vge)|*.vge|File (*.svg)|*.svg|File (*.png)|*.png|File (*.jpg)|*.jpg|File (*.emf)|*.emf|File (*.gif)|*.gif|File (*.ico)|*.ico|File (*.tif)|*.tif|File (*.bmp)|*.bmp";


            if (saveEditorFileDialog.ShowDialog(frm) != DialogResult.OK) return;
            switch (saveEditorFileDialog.FilterIndex)
            {
                case 1: //file VGE cryptato (creato da questo programma)
                    _fileName = saveEditorFileDialog.FileName;
                    SaverLoader.SaveToFile(_fileName, _layer);
                    UndoRedoManager.Instance.ClearHistory();
                    frm.Text = pCaption + @" - " + _fileName;
                    BuildInterface();
                    break;
                case 2: //file SVG non cryptato
                    ExportImport.ExportToSvg(saveEditorFileDialog.FileName, _layer);
                    break;
                default:
                    ExportImport.SaveToImage(saveEditorFileDialog.FileName, _layer);
                    break;
            }
        }
        #endregion MENU_FILE

        #region MENU_EDIT
        public void Undo()
        {
            _selectionController.Clear();
            UndoRedoManager.Instance.Undo();
            UpdateInterface();
        }

        public void Redo()
        {
            _selectionController.Clear();
            UndoRedoManager.Instance.Redo();
            UpdateInterface();
        }

        public void DuplicateFigure()
        {
            var exists = GetSelectionControllerSelection.Count > 0;
            if (!exists) return;
            OnLayerStartChanging("Duplicate Selected Figures");
            var list = new List<Figure>();
            foreach (var fig in GetSelectionControllerSelection.Select(figure => figure.DeepClone()))
            {
                fig.Transform.Matrix.Translate(10, 10, MatrixOrder.Append);
                list.Add(fig);
                _layer.Figures.Add(fig);
            }
            OnLayerChanged();
            _selectionController.Clear();
            foreach (var fig in list)
                GetSelectionControllerSelection.Add(fig);
            _selectionController.UpdateMarkers();
            BuildInterface();
        }

        public void Paste()
        {
            if (!_selectionController.CanPasteFromClipboard) return;
            _selectionController.PasteFromClipboardAndSelected();
        }

        public void PasteAtCanvasCenter()
        {
            if (!_selectionController.CanPasteFromClipboard) return;
            var scaleFactor = _selectionController.ScaleFactor;
            if (scaleFactor <= 0) scaleFactor = 1f;
            var center = new PointF(pbCanvas.ClientSize.Width / (2f * scaleFactor),
                pbCanvas.ClientSize.Height / (2f * scaleFactor));
            _selectionController.PasteFromClipboardAt(center);
        }

        public void PasteAtCanvasOrigin()
        {
            if (!_selectionController.CanPasteFromClipboard) return;
            _selectionController.PasteFromClipboardAtOrigin();
        }

        public void Copy()
        {
            _selectionController.CopySelectedToClipboard();
        }

        public void CopySelectedBasketObjectsToMemory()
        {
            _selectionController.CopySelectedBasketObjectsToMemory();
        }

        /// <summary>Copies selected basket objects from this action set's canvas to Windows clipboard.</summary>
        public void CopySelectedBasketObjectsToClipboard()
        {
            _selectionController.CopySelectedBasketObjectsToClipboard();
        }

        /// <summary>Pastes basket objects from Windows clipboard into this action set's canvas/layer.</summary>
        public void PasteBasketObjectsFromClipboard()
        {
            _selectionController.PasteBasketObjectsFromClipboard();
            BuildInterface();
        }

        public bool CanPasteBasketObjectsFromMemory
        {
            get { return _selectionController.CanPasteBasketObjectsFromMemory; }
        }

        public void PasteBasketObjectsFromMemory()
        {
            _selectionController.PasteBasketObjectsFromMemory();
            BuildInterface();
        }

        public void Cut()
        {
            _selectionController.CutSelectedToClipboard();
        }

        public void Delete()
        {
            var exists = GetSelectionControllerSelection.Count > 0;
            if (!exists) return;
            OnLayerStartChanging("Delete Selected Figures");
            foreach (var figure in GetSelectionControllerSelection)
            {
                _layer.Figures.Remove(figure);
                foreach (var layer in GetLayerItems)
                    if (layer.Figures.Contains(figure))
                        layer.Figures.Remove(figure);
            }
            OnLayerChanged();
            _selectionController.Clear();
            BuildInterface();
        }

        public  void SelectAll()
        {
            SelectionControlSelectAll();
        }

        public void MoveUp(Keys ModifierKeys)
        {
            _selectionController.MoveByKeys(0, -1, ModifierKeys);
        }

        public void MoveDown(Keys ModifierKeys)
        {
            _selectionController.MoveByKeys(0, 1, ModifierKeys);
        }

        public void MoveLeft(Keys ModifierKeys)
        {
            _selectionController.MoveByKeys(-1, 0, ModifierKeys);
        }

        public void MoveRight(Keys ModifierKeys)
        {
            _selectionController.MoveByKeys(1, 0, ModifierKeys);
        }

        #endregion MENU_EDIT

        public void SetSelectionContainerGeometryTitle(string pTitle)
        {
            //_selectionController.Selection.Geometry.Title = pTitle;

            if (_selectionController.Selection != null && _selectionController.Selection.Count == 1)
            {
                if (_selectionController.Selection.Count == 1)
                {
                    _selectionController.Selection.CurrentFigure.Geometry.Title = pTitle;
                  


                }
                else
                    MessageBox.Show("Impossibile continuare, devi selezionare soltanto una figura!", "Scrittura del Ttolo delal figura");


            }
            else
                MessageBox.Show("Impossibile continuare, nessuna selezione!", "Scrittura del Ttolo delal figura");

           
        }

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public void SelectionSetFillStyle (object valore)
        {
            if (_selectionController.Selection != null)
            {
                // fire event
                StartChanging(this, new ChangingEventArgs("Fill Style"));

                // get list of objects
                var fillStyles = _selectionController.Selection.Select(f => f.Style.FillStyle).ToList();

                if (valore is bool boolValore)
                {
                    fillStyles.SetProperty(f => f.IsVisible = boolValore);
                }
                else if (valore is int numeroValore)
                {
                    fillStyles.SetProperty(f => f.Opacity = numeroValore);

                }
                else if (valore is Color coloreValore)
                {
                    fillStyles.SetProperty(f => f.Color = coloreValore);
                }
            }

           

         

        }

    
  

    }
    
}


