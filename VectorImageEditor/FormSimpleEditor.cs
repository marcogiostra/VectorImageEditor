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
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows.Forms;
using VectorImageEditor.Common;
using VectorImageEditor.Controllers;
using VectorImageEditor.Controls;
using VectorImageEditor.Controls.AltreAggiunte;
using VectorImageEditor.Controls.Basket;
using VectorImageEditor.DevWithGiosh;
using VectorImageEditor.EditorLayersInterface;
using VectorImageEditor.Static;

namespace VectorImageEditor
{
    public partial class FormVectorImageEditor : Form
    {
        #region DICHIARAZIONI

        #region MG
        MacroActions MAs = new MacroActions();
        #endregion MG

        readonly string _caption;
        readonly VersionInfo _versionInfo;

        BackgroundType _backgroundMode = BackgroundType.Chess;


        private MacroGeometrySelectionSwitchMode geometrySwitchSelected = MacroGeometrySelectionSwitchMode.None;
        private MacroFillSelection fillSelected = MacroFillSelection.SolidBrush;
        private MacroEffectSelection effectSelected = MacroEffectSelection.None;

        private ToolStripMenuItem _effectRenderer;
        private ToolStripMenuItem _switchGeometry;
        private ToolStripMenuItem _switchBrush;
        private ToolStripMenuItem _copyBasketObjectsToMemory;
        private ToolStripMenuItem _pasteBasketObjectsFromMemory;
        private ToolStripMenuItem _pasteClipboardAtCenter;
        private ToolStripMenuItem _pasteClipboardAtOrigin;
        private bool _suppressCanvasContextMenu;

        #endregion DICHIARAZIONI

        #region CLASSE
        public FormVectorImageEditor()
        {
            InitializeComponent();
            _versionInfo = new VersionInfo();
            _caption = string.Format("Simple Vector Graphics Editor (Ver {0:0.0})",
                (decimal)_versionInfo.Version / 10);

            ConnectEditors();

            MAs = new MacroActions(panelForScroll, pbCanvas);
            BuildSvgObjectInsertMenu();
            BuildBasketObjectMemoryMenu();

            MAs.ControlEnabledChanged += MacroAction_ControlEnabledChanged;
            MAs.EventUpdateFiguresTree += MacroAction_EventUpdateFiguresTree;
            MAs.EventUpdateInterface += MacroAction_EventUpdateInterface;
            MAs.EventBuildInterface += MacroActions_EventBuildInterface;
            MAs.EventGeometySwitcherd += MacroAction_EventGeometySwitcherd;
            MAs.EventInfoSelection += MacroAction_EventInfoSelection;
            MAs.EventFillSelection += MacroAction_EventFillSelection;
            MAs.EventEffectSelection += MacroAction_EventEffectSelection;

        }

        private void FormVectorImageEditor_Load(object sender, EventArgs e)
        {
            Text = _caption;
        }

        private void FormVectorImageEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!MAs.FileChanged) return;
            var result = MessageBox.Show(this, @"Il documento contiene dati non salvati." + Environment.NewLine +
                @"Vuoi salvarli?", @"Chiusura dell'applicazione",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            if (result == DialogResult.Yes)
                tsmSave.PerformClick();
            else if (result == DialogResult.Cancel)
                e.Cancel = true;
        }

        #endregion CLASSE

        #region EVENTS
        private void MacroAction_EventUpdateFiguresTree(object sender, EventArgs e)
        {
            //aggiorna il treeview
            UpdateFiguresTree();
        }

        public void MacroAction_ControlEnabledChanged(object sender, ControlStateChangedEventArgs e)
        {
            string nome = e.NomeControl.ToString();
            bool state = e.IsEnabled;
            foreach (Control item in this.Controls)
            {
                if (item.Name == nome)
                {
                    item.Enabled = state;
                    break;
                }
            }
        }

        private void MacroAction_EventUpdateInterface(object sender, EventArgs e)
        {
            UpdateInterface();
        }

        private void MacroActions_EventBuildInterface(object sender, EventArgs e)
        {
            BuildInterface();
        }

        private void MacroAction_EventInfoSelection(object sender, string e)
        {
            tsslRibbonRect.Text = e;
        }

        private void MacroAction_EventGeometySwitcherd(object sender, string e)
        {
            tsddbGeometySwitcher.Text = e;
        }

        private void MacroAction_EventFillSelection(object sender, string e)
        {
            tsddbFillBrushSwitcher.Text = e;
        }
        private void MacroAction_EventEffectSelection(object sender, string e)
        {
            tsddbEffectSwitcher.Text = e;
        }


        #endregion Events

        #region f()_EVENTS
        /// <summary>
        /// Aggiorna il treeview dei layers e delle figure
        /// </summary>
        private void UpdateFiguresTree()
        {
            var first = MAs.GetSelectionControllerSelection.FirstOrDefault();

            try
            {
                tvFigures.AfterSelect -= tvFigures_AfterSelect;
                tvFigures.BeginUpdate();
                try
                {
                    tvFigures.Nodes.Clear();
                    var noLayers = new TreeNode("(no layers)");
                    tvFigures.Nodes.Add(noLayers);
                    AddToTree(noLayers.Nodes, null, first);
                    foreach (var layer in MAs.GetLayerItems.OrderBy(layer => layer.Name).Where(layer => layer.Figures.Count > 0))
                    {
                        var nodeLayer = new TreeNode(layer.Name);
                        tvFigures.Nodes.Add(nodeLayer);
                        AddToTree(nodeLayer.Nodes, layer, first);
                    }
                }
                finally
                {
                    tvFigures.EndUpdate();
                }
            }
            finally
            {
                if (tvFigures.SelectedNode != null)
                    tvFigures.SelectedNode.EnsureVisible();
                tvFigures.AfterSelect += tvFigures_AfterSelect;
            }
        }

        /// <summary>
        /// Aggiorna l'interfaccia utente in base allo stato corrente del controller di selezione e delle figure.
        /// </summary>
        private void UpdateInterface()
        {
            tsbPolyline.Enabled = tsbRect.Enabled = tsbRomb.Enabled = tsbText.Enabled = tsbPicture.Enabled = MAs.SelectionControllerEditorMode == EditorMode.Select;

            tsbSelectMode.Checked = MAs.SelectionControllerEditorMode == EditorMode.Select;
            tsbSkewMode.Checked = MAs.SelectionControllerEditorMode == EditorMode.Skew;
            tsbWarpMode.Checked = MAs.SelectionControllerEditorMode == EditorMode.Warp;
            tsbVertexMode.Checked = MAs.SelectionControllerEditorMode == EditorMode.Verticies;

            var changedUndo = MAs.FileChanged != UndoRedoManager.Instance.CanUndo;
            tsbUndo.Enabled = tsmUndo.Enabled = MAs.FileChanged = UndoRedoManager.Instance.CanUndo;
            var changedRedo = tsbRedo.Enabled != UndoRedoManager.Instance.CanRedo;
            tsbRedo.Enabled = tsmRedo.Enabled = UndoRedoManager.Instance.CanRedo;
            tsbSave.Enabled = tsmSave.Enabled = MAs.FileChanged;
            tsslEditorMode.Text = string.Format("Mode: {0}", MAs._selectionController.EditorMode);
            var exists = MAs.GetSelectionControllerSelection.ForAll(f =>
                                 f.Geometry is PrimitiveGeometry && f.Renderer is DefaultRenderer);
            tsddbGeometySwitcher.Enabled = exists;
            tsddbFillBrushSwitcher.Enabled = tsddbEffectSwitcher.Enabled = tsmDuplicateFigure.Enabled = tsmiDuplicateFigure.Enabled =
                 tsbFlipX.Enabled = tsbFlipY.Enabled = tsbRotate90Ccw.Enabled = tsbRotate90Cw.Enabled = tsbRotate180.Enabled =
                 tsbCopy.Enabled = tsmCopy.Enabled = tsbCut.Enabled = tsmCut.Enabled = tsmDelete.Enabled = tsmiDelete.Enabled =
                    tsbBringToFront.Enabled = tsbSendToBack.Enabled = tsbUpToFront.Enabled = tsbSendToDown.Enabled =
                    tsmiAssignedToLayer.Enabled = tsbDublicate.Enabled = MAs.GetSelectionControllerSelection.Count > 0;

            tsbAlignLeft.Enabled = tsbAlignCenter.Enabled = tsbAlignRight.Enabled =
                 tsbAlignTop.Enabled = tsbAlignMiddle.Enabled = tsbAlignBottom.Enabled = MAs.GetSelectionControllerSelection.Count > 1;
            tsbEvenHorizontalSpaces.Enabled = tsbEvenVerticalSpaces.Enabled = MAs.GetSelectionControllerSelection.Count > 2;
            tsbSameWidth.Enabled = tsbSameHeight.Enabled = tsbSameBothSizes.Enabled =
                         MAs.GetSelectionControllerSelection.Count(SelectionHelper.IsNotSkewAndRotated) > 1;

            tsbConvertToPath.Enabled = MAs.GetSelectionControllerSelection.Count(fig =>
                        fig.Geometry.AllowedOperations.HasFlag(AllowedOperations.Pathed)) > 0;

            if (changedUndo || changedRedo)
                UpdateFiguresTree();

            try
            {
                cbScaleFactor.SelectedIndexChanged -= cbScaleFactor_SelectedIndexChanged;
                cbScaleFactor.Text = string.Format("{0}%", MAs._selectionController.ScaleFactor * 100);
            }
            finally
            {
                cbScaleFactor.SelectedIndexChanged += cbScaleFactor_SelectedIndexChanged;
            }

            pbCanvas.Invalidate();
        }

        /// <summary>
        /// Crea l'interfaccia
        /// </summary>
        void BuildInterface()
        {
            //build tools
            foreach (var editor in pnTools.Controls.OfType<IEditor<LayerSelectionInfo>>()) //get editors of layer
                editor.Build(new LayerSelectionInfo { Layer = MAs._layer, Selection = MAs.GetSelectionControllerSelection });

            foreach (var editor in pnTools.Controls.OfType<IEditor<Selection>>()) //get editors of figure
                editor.Build(MAs.GetSelectionControllerSelection);

            // Player options belong to the currently selected player. Keep them at
            // the top of the scrolling tools panel, otherwise they end up after all
            // the general style editors and appear to be missing.
            var numberEditor = pnTools.Controls.OfType<PlayerNumberEditor>().FirstOrDefault();
            var shapeEditor = pnTools.Controls.OfType<UserControl>().FirstOrDefault(editor =>
                editor.Visible && (editor is RoundAttackPlayerEditor || editor is GiocatoreStilizzatoEditor ||
                                   editor is GiocatoreEditor || editor is OggettoBasketEditor || editor is BallBasketEditor));
            if (numberEditor != null && numberEditor.Visible)
            {
                pnTools.Controls.SetChildIndex(numberEditor, 0);
                if (shapeEditor != null) pnTools.Controls.SetChildIndex(shapeEditor, 1);
                pnTools.ScrollControlIntoView(numberEditor);
            }
            else if (shapeEditor != null)
            {
                pnTools.Controls.SetChildIndex(shapeEditor, 0);
                pnTools.ScrollControlIntoView(shapeEditor);
            }
            //
            UpdateFiguresTree();
            UpdateInterface();
        }
        #endregion f()_EVENTS

        #region f()
        private void ConnectEditors()
        {
            pnTools.Controls.Clear();
            var editors = new[]
            {
                typeof(LayerStyleEditor),
                typeof(FillStyleEditor),
                typeof(BorderStyleEditor),
                typeof(ShadowStyleEditor),
                typeof(GlowStyleEditor),
                typeof(GradientStyleEditor),
                typeof(HatchStyleEditor),
                typeof(TextureStyleEditor),
                typeof(PolygonStyleEditor),
                typeof(BezierStyleEditor),
                typeof(TextStyleEditor),
                typeof(TextBlockStyleEditor),
                typeof(ImageStyleEditor),
                //typeof(GroupStyleEditor),
                typeof(WedgeStyleEditor),
                typeof(ArrowStyleEditor),
                typeof(SvgStyleEditor),
                typeof(PlayerNumberEditor),
                typeof(RoundAttackPlayerEditor),
                typeof(GiocatoreStilizzatoEditor),
                typeof(GiocatoreEditor),
                typeof(OggettoBasketEditor),
                typeof(BallBasketEditor)
            };
            foreach (var typeName in editors)
            {
                var uc = (UserControl)Activator.CreateInstance(typeName);
                var figEditor = uc as VectorImageEditor.Controls.IEditor<Selection>;
                if (figEditor != null)
                {
                    figEditor.StartChanging += pnStyle_StartChanging;
                    figEditor.Changed += pnStyle_Changed;
                }
                var lyaerEditor = uc as IEditor<LayerSelectionInfo>;
                if (lyaerEditor != null)
                {
                    lyaerEditor.StartChanging += pnStyle_StartChanging;
                    lyaerEditor.Changed += pnStyle_Changed;
                }
                pnTools.Controls.Add(uc);
            }
        }
        private void AddToTree(TreeNodeCollection nodes, LayerItem layer, Figure first)
        {
            foreach (var fig in MAs._layer.Figures.ToList().AsEnumerable().Reverse())
            {
                if (layer == null && !MAs._layer.AssignedToLayer(fig) ||
                    layer != null && layer.Figures.Contains(fig))
                {
                    var state = !MAs._layer.IsVisible(fig)
                        ? " (hidden)"
                        : MAs._layer.IsLocked(fig) ? " (locked)" : "";
                    var figureName = fig.Geometry.Name != null &&
                        fig.Geometry.Name.StartsWith("Giocatore", StringComparison.Ordinal)
                        ? fig.Geometry.Name
                        : fig.Geometry.ToString();
                    var fignode = new FigureTreeNode(
                        string.Format("{0}{1}", figureName, state))
                    { Figure = fig };
                    nodes.Add(fignode);
                    AddDecorators(fig, fignode);
                    if (fig == first)
                    {
                        tvFigures.SelectedNode = fignode;
                        fignode.ExpandAll();
                    }
                }
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BuildInterface();
        }
        private static void AddDecorators(Figure fig, FigureTreeNode fignode)
        {
            if (Helper.ContainsAnyDecorator(fig))
            {
                if (FillDecorator.ContainsAnyDecorator(fig.Style.FillStyle))
                {
                    var list = FillDecorator.GetDecorators(fig.Style.FillStyle);
                    foreach (var item in list)
                        fignode.Nodes.Add(string.Format("{0}FillStyleDecorator", item));
                }
                if (RendererDecorator.ContainsAnyDecorator(fig.Renderer))
                {
                    var list = RendererDecorator.GetDecorators(fig.Renderer);
                    foreach (var item in list)
                        fignode.Nodes.Add(string.Format("{0}RendererDecorator", item));
                }
            }
        }
        #endregion f()

        #region CANVAS
        private void pbCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            _suppressCanvasContextMenu = e.Button == MouseButtons.Right &&
                MAs._selectionController.Selection.Geometry is EditorModel.Geometry.AddLineGeometry;
            MAs.Canvas_MouseDown(e, ModifierKeys);
        }

        private void pbCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            MAs.Canvas_MouseMove(e, ModifierKeys);
        }

        private void pbCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            MAs.Canvas_MouseUp(e, ModifierKeys);
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            MAs.Canvas_Paint(e);
        }
        private void panelForScroll_SizeChanged(object sender, EventArgs e)
        {
            MAs.UpdateCanvasSize();
        }
        #region DRAG_AND_DROP
        private void panelForScroll_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                var allowed = new[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif" };
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && allowed.Contains(Path.GetExtension(files[0]).ToLower()))
                    e.Effect = DragDropEffects.All;
                else
                    e.Effect = DragDropEffects.None;
            }
        }


        private void panelForScroll_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1) return;
            MAs.ConvasZoneForScroll_DragDrop(files, e);
        }
        #endregion DRAG_AND_DROP
        #endregion CANVAS

        #region OPERAZIONI_SELECTED_CONTROLLER


        private void tsmiFlipX_Click(object sender, EventArgs e)
        {
            MAs._selectionController.FlipX();
            UpdateInterface();
        }

        private void tsmiFlipY_Click(object sender, EventArgs e)
        {
            MAs._selectionController.FlipY();
            UpdateInterface();
        }

        private void tsmiRotate90Cw_Click(object sender, EventArgs e)
        {
            MAs._selectionController.Rotate90Cw();
            UpdateInterface();
        }

        private void tsmiRotate90Ccw_Click(object sender, EventArgs e)
        {
            MAs._selectionController.Rotate90Ccw();
            UpdateInterface();
        }

        private void tsmiRotate180_Click(object sender, EventArgs e)
        {
            MAs._selectionController.Rotate180();
            UpdateInterface();
        }

        private void tsmiBringToFront_Click(object sender, EventArgs e)
        {
            MAs._selectionController.BringToFront();
            UpdateInterface();
        }

        private void tsbUpToFront_Click(object sender, EventArgs e)
        {
            MAs._selectionController.UpToFront();
            UpdateInterface();
        }

        private void tsbSendToDown_Click(object sender, EventArgs e)
        {
            MAs._selectionController.SendToDown();
            UpdateInterface();
        }

        private void tsmiSendToBack_Click(object sender, EventArgs e)
        {
            MAs._selectionController.SendToBack();
            UpdateInterface();
        }
        #endregion OPERAZIONI_SELECTED_CONTROLLER

        #region EVENT_LEGATE_LAYER
        private void pnStyle_StartChanging(object sender, ChangingEventArgs e)
        {
            MAs.OnLayerStartChanging(e.ChangingName);
        }

        private void pnStyle_Changed(object sender, EventArgs e)
        {
            MAs.StyleChanaged();
        }
        #endregion EVENT_LEGATE_LAYER

        #region SWITCHER_GEOMETRY
        private void tsddbGeometySwitcher_ButtonClick(object sender, EventArgs e)
        {

            if (_switchGeometry is null)
                tsddbGeometySwitcher.ShowDropDown();
            else
            {
                string tempTAG = string.Empty;
                if (_switchGeometry.Tag != null)
                    tempTAG = _switchGeometry.Tag.ToString();

                MAs.ChangePrimitiveGeometry(sender, geometrySwitchSelected, tempTAG);
            }


        }
        private void tsmiPrimitiveGeometry_Click(object sender, EventArgs e)
        {

            _switchGeometry = (ToolStripMenuItem)sender;
            //determina il nome effettivo
            string nameSwitcher = MyStringHelper.RimuoviMeno(MyStringHelper.RimuoviSpazi(_switchGeometry.Text));
            geometrySwitchSelected = MAs.GetGemotrySwitchSelected(nameSwitcher);
            string tempTAG = string.Empty;
            if (_switchGeometry.Tag != null)
                tempTAG = _switchGeometry.Tag.ToString();
            MAs.ChangePrimitiveGeometry(sender, geometrySwitchSelected, tempTAG);

        }
        #endregion SWITCHER_GEOMETRY

        #region FILL_BRUSH

        private void tsddbFillBrushSwitcher_Click(object sender, EventArgs e)
        {
            if (_switchBrush == null)
                tsddbFillBrushSwitcher.ShowDropDown();
            else
                MAs.ChangeFillBrush(sender, fillSelected);
        }
        private void tsmiSolidBrush_Click(object sender, EventArgs e)
        {
            _switchBrush = (ToolStripMenuItem)sender;
            //determina il nome effettivo
            string name = _switchBrush.Text;
            fillSelected = MAs.GetFillSelected(name);
            MAs.ChangeFillBrush(sender, fillSelected);
        }
        #endregion FILL_BRUSH

        #region EFFECT

        private void tsddbEffectSwitcher_ButtonClick(object sender, EventArgs e)
        {
            if (_effectRenderer == null)
                tsddbEffectSwitcher.ShowDropDown();
            else
                MAs.ChangeEffects(sender, effectSelected);

        }

        private void tsmiNoneEffects_Click(object sender, EventArgs e)
        {
            _effectRenderer = (ToolStripMenuItem)sender;
            //determina il nome effettivo
            string name = _effectRenderer.Text;
            effectSelected = MAs.GetEffectSelected(name);
            MAs.ChangeEffects(sender, effectSelected);
        }
        #endregion EFFECT

        #region MENU_FILE
        private void tsmCreate_Click(object sender, EventArgs e)
        {
            MAs.CreateNew(this, _caption);
        }

        private void tsmOpen_Click(object sender, EventArgs e)
        {
            MAs.Open(this, _caption);
        }

        private void tsmImportSvg_Click(object sender, EventArgs e)
        {
            MAs.ImportSvg(this);
        }

        private void tsmImportSvgFigures_Click(object sender, EventArgs e)
        {
            MAs.ImportSvgFigures(this);
        }

        private void tsmImportSvgResourceFigures_Click(object sender, EventArgs e)
        {
            MAs.ImportSvgResourceFigures(this);
        }

        private void tsmGenerateSvgToMemory_Click(object sender, EventArgs e)
        {
            GenerateImportedSvg(SvgGenerationTarget.ApplicationMemory);
        }

        private void tsmGenerateSvgToClipboard_Click(object sender, EventArgs e)
        {
            GenerateImportedSvg(SvgGenerationTarget.WindowsClipboard);
        }

        private void GenerateImportedSvg(SvgGenerationTarget target)
        {
            try
            {
                MAs.GenerateSvgFromImportedFigures(target);
                var message = target == SvgGenerationTarget.WindowsClipboard
                    ? "SVG generato e copiato negli appunti."
                    : "SVG generato e conservato in memoria nell'applicazione.";
                MessageBox.Show(this, message, "Genera SVG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Impossibile generare l'SVG." + Environment.NewLine + ex.Message,
                    "Genera SVG", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildSvgObjectInsertMenu()
        {
            var insertMenu = new ToolStripMenuItem("Inserisci SVG generato come...");
            foreach (SvgImportSource source in Enum.GetValues(typeof(SvgImportSource)))
            {
                var sourceMenu = new ToolStripMenuItem(source == SvgImportSource.ApplicationMemory
                    ? "Dalla memoria" : "Dagli appunti");
                var sourceValue = source;
                sourceMenu.Click += (sender, e) => InsertGeneratedSvg(sourceValue);
                insertMenu.DropDownItems.Add(sourceMenu);
            }

            var insertIndex = tsmFileMenu.DropDownItems.IndexOf(tsmGenerateSvgFigures) + 1;
            tsmFileMenu.DropDownItems.Insert(insertIndex, insertMenu);
        }

        private void BuildBasketObjectMemoryMenu()
        {
            _pasteClipboardAtCenter = new ToolStripMenuItem("Paste al centro del canvas");
            _pasteClipboardAtCenter.Click += (sender, e) => MAs.PasteAtCanvasCenter();
            _pasteClipboardAtOrigin = new ToolStripMenuItem("Paste al punto di origine del canvas");
            _pasteClipboardAtOrigin.Click += (sender, e) => MAs.PasteAtCanvasOrigin();
            _copyBasketObjectsToMemory = new ToolStripMenuItem("Copia oggetti basket in memoria");
            _copyBasketObjectsToMemory.Click += (sender, e) => MAs.CopySelectedBasketObjectsToMemory();
            _pasteBasketObjectsFromMemory = new ToolStripMenuItem("Incolla oggetti basket dalla memoria");
            _pasteBasketObjectsFromMemory.Click += (sender, e) => MAs.PasteBasketObjectsFromMemory();

            var insertIndex = tsmEditMenu.DropDownItems.IndexOf(tsmPaste) + 1;
            tsmEditMenu.DropDownItems.Insert(insertIndex++, _pasteClipboardAtCenter);
            tsmEditMenu.DropDownItems.Insert(insertIndex++, _pasteClipboardAtOrigin);
            tsmEditMenu.DropDownItems.Insert(insertIndex++, new ToolStripSeparator());
            tsmEditMenu.DropDownItems.Insert(insertIndex++, _copyBasketObjectsToMemory);
            tsmEditMenu.DropDownItems.Insert(insertIndex, _pasteBasketObjectsFromMemory);
            _copyBasketObjectsToMemory.Enabled = false;
            _pasteBasketObjectsFromMemory.Enabled = false;
        }

        private void InsertGeneratedSvg(SvgImportSource source)
        {
            try
            {
                MAs.InsertGeneratedSvg(source);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Impossibile inserire l'SVG." + Environment.NewLine + ex.Message,
                    "Inserisci SVG", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            MAs.Save(this, _caption);
        }

        /// <summary>
        /// COMAND: SalveAs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmSaveAs_Click(object sender, EventArgs e)
        {
            MAs.SaveAs(this, _caption);
        }

        private void tsmExit_Click(object sender, EventArgs e)
        {
            Close();
            MAs = null;
        }


        #endregion EMNU_FILE

        #region MENU_EDIT
        private void tsmUndo_Click(object sender, EventArgs e)
        {
            MAs.Undo();
        }


        private void tsmRedo_Click(object sender, EventArgs e)
        {
            MAs.Redo();
        }

        private void tsmDuplicateFigure_Click(object sender, EventArgs e)
        {
            MAs.DuplicateFigure();
        }

        private void tsmPaste_Click(object sender, EventArgs e)
        {
            MAs.Paste();
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
            MAs.Copy();
        }

        private void tsbCut_Click(object sender, EventArgs e)
        {
            MAs.Cut();
        }
        private void tsmDelete_Click(object sender, EventArgs e)
        {
            MAs.Delete();
        }

        private void tsmSelectAll_Click(object sender, EventArgs e)
        {
            MAs.SelectAll();
        }

        private void tsmiMoveUp_Click(object sender, EventArgs e)
        {
            MAs.MoveUp(ModifierKeys);
        }

        private void tsmiMoveDown_Click(object sender, EventArgs e)
        {
            MAs.MoveDown(ModifierKeys);
        }

        private void tsmiMoveLeft_Click(object sender, EventArgs e)
        {
            MAs.MoveLeft(ModifierKeys);
        }

        private void tsmiMoveRight_Click(object sender, EventArgs e)
        {
            MAs.MoveRight(ModifierKeys);
        }


        #endregion MENU_EDIT

        #region TOOLBAR_VERTICALE
        /// <summary>
        /// Crea una FIGURA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateFigure_Click(object sender, EventArgs e)
        {
            if (sender == null) return;

            //imposta la figura selezionata e pulisce _selectionController
            string nomeSender = sender.ToString();
            if (!string.IsNullOrEmpty(sender.ToString()))
            {
                if (nomeSender == "Regular4")
                    nomeSender = "Romb";

                if (nomeSender == "Text" && sender.ToString() == "tsbText")
                    nomeSender = "TextBlock";

                if (nomeSender == "InsertImage")
                    nomeSender = "Picture";

                MAs.SetGeometrySelected(nomeSender);

                //crea la figura
                MAs.CreateFigure(MAs.GeometryModeSelected);
            }
            else if (sender is ToolStripMenuItem)
            {
                var name = (sender as ToolStripMenuItem).Text;
                if (name.StartsWith("Regular"))
                {
                    MAs.SetGeometrySelected(name);

                    //crea la figura
                    MAs.CreateFigure(MAs.GeometryModeSelected);
                }
            }


        }

        /// <summary>
        /// gestioni dei metodi di selezioni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeSelector_Click(object sender, EventArgs e)
        {
            if (sender == tsbSelectMode)
                MAs.SelectionControllerEditorMode = EditorMode.Select;
            else if (sender == tsbSkewMode)
                MAs.SelectionControllerEditorMode = EditorMode.Skew;
            else if (sender == tsbVertexMode)
                MAs.SelectionControllerEditorMode = EditorMode.Verticies;
            else if (sender == tsbWarpMode)
                MAs.SelectionControllerEditorMode = EditorMode.Warp;
        }
        #endregion TOOLBAR_VERTICALE

        #region TOOLBAR_ORIZZONTALI

        private void tsbAlignLeft_Click(object sender, EventArgs e)
        {
            MAs.SelectionAlignHorizontal(FigureAlignment.Near);
        }

        private void tsbAlignCenter_Click(object sender, EventArgs e)
        {
            MAs.SelectionAlignHorizontal(FigureAlignment.Center);
        }

        private void tsbAlignRight_Click(object sender, EventArgs e)
        {
            MAs.SelectionAlignHorizontal(FigureAlignment.Far);
        }

        private void tsbAlignTop_Click(object sender, EventArgs e)
        {
            MAs.SelectionAlignVertical(FigureAlignment.Near);

        }

        private void tsbAlignMiddle_Click(object sender, EventArgs e)
        {
            MAs.SelectionAlignVertical(FigureAlignment.Center);

        }

        private void tsbAlignBottom_Click(object sender, EventArgs e)
        {
            MAs.SelectionAlignVertical(FigureAlignment.Far);

        }

        private void tsbSameBothSizes_Click(object sender, EventArgs e)
        {
            MAs.SelectionSameResize(FigureResize.Both);
        }

        private void tsbSameWidth_Click(object sender, EventArgs e)
        {
            MAs.SelectionSameResize(FigureResize.Width);
        }

        private void tsbSameHeight_Click(object sender, EventArgs e)
        {
            MAs.SelectionSameResize(FigureResize.Heigth);
        }

        private void tsbEvenHorizontalSpaces_Click(object sender, EventArgs e)
        {
            MAs.SelectionEvenSpaces(FigureArrange.Horizontal);
        }

        private void tsbEvenVerticalSpaces_Click(object sender, EventArgs e)
        {
            MAs.SelectionEvenSpaces(FigureArrange.Vertical);

        }

        /// <summary>
        /// gestione della percentuale di ingrandimento/riduzione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbScaleFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pst = float.Parse(cbScaleFactor.Text.TrimEnd('%'));
            MAs.SelectionControlerScalerFactor(pst);
        }

        private void tsmiLayerProperty_Click(object sender, EventArgs e)
        {
            //apre la finestra di dialogo per le proprietà del layer
            var frm = new FormLayersProperty();
            frm.StartChanging += pnStyle_StartChanging;
            frm.Changed += pnStyle_Changed;
            try
            {
                frm.Build(MAs._layer, MAs.GetSelectionControllerSelection);
                frm.ShowDialog(this);
            }
            finally
            {
                frm.Changed -= pnStyle_Changed;
                frm.StartChanging -= pnStyle_StartChanging;
            }
        }
        private void tsmiAssignedToLayer_Click(object sender, EventArgs e)
        {
            //chiama il form per la selezione dei layer
            var frm = new FormLayerLashing();
            frm.StartChanging += pnStyle_StartChanging;
            frm.Changed += pnStyle_Changed;
            try
            {
                frm.Build(MAs._layer, MAs.GetSelectionControllerSelection);
                frm.ShowDialog(this);
            }
            finally
            {
                frm.Changed -= pnStyle_Changed;
                frm.StartChanging -= pnStyle_StartChanging;
            }
        }


        private void tsbLayers_ButtonClick(object sender, EventArgs e)
        {
            tsbLayers.ShowDropDown();
        }


        private void tsbBackground_Click(object sender, EventArgs e)
        {
            var res = ((int)_backgroundMode + 1) % Enum.GetValues(typeof(BackgroundType)).Length;
            _backgroundMode = (BackgroundType)res;
            var colors = new Color[] { Color.Transparent, Color.White, Color.Black, Color.Gray, Color.Transparent };
            panelForScroll.BackColor = colors[res];
            panelForScroll.BackgroundImage = _backgroundMode == BackgroundType.Chess ? Properties.Resources.transparent1 : null;
        }
    
        #endregion TOOLBAR_ORIZZONTALI

        private void tvFigures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //pulisce la selezione del contorller (nessun oggetto selezionsato)
            MAs.GetSelectionControllerSelection.Clear();

            if (e.Node != null)
            {
                var fignode = e.Node as FigureTreeNode;
                if (fignode == null) return;
                if (!MAs._layer.IsVisible(fignode.Figure) || MAs._layer.IsLocked(fignode.Figure)) return;
                MAs.GetSelectionControllerSelection.Add(fignode.Figure);
                MAs._selectionController.UpdateMarkers();

                BuildInterface();
            }
        }

        private void timerCheckClipboard_Tick(object sender, EventArgs e)
        {
            tsbPaste.Enabled = tsmPaste.Enabled = tsmiPaste.Enabled = MAs._selectionController.CanPasteFromClipboard;
            if (_pasteClipboardAtCenter != null)
                _pasteClipboardAtCenter.Enabled = MAs._selectionController.CanPasteFromClipboard;
            if (_pasteClipboardAtOrigin != null)
                _pasteClipboardAtOrigin.Enabled = MAs._selectionController.CanPasteFromClipboard;
        }

        private void cmsCanvasPopup_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _suppressCanvasContextMenu;
            _suppressCanvasContextMenu = false;
        }

        private void tsmEditMenu_DropDownOpening(object sender, EventArgs e)
        {
            var hasBasketObjects = MAs.GetSelectionControllerSelection.Any(figure =>
                figure.Geometry != null &&
                (figure.Geometry.Name == "Oggetto" || figure.Geometry.Name == "GiocatoreAttacco" ||
                 figure.Geometry.Name == "GiocatoreDifesa"));
            _copyBasketObjectsToMemory.Enabled = hasBasketObjects;
            _pasteBasketObjectsFromMemory.Enabled = MAs.CanPasteBasketObjectsFromMemory;
            _pasteClipboardAtCenter.Enabled = MAs._selectionController.CanPasteFromClipboard;
            _pasteClipboardAtOrigin.Enabled = MAs._selectionController.CanPasteFromClipboard;
        }


        private void tsbConvertToPath_Click(object sender, EventArgs e)
        {
            MAs.SelectionConvertToPath();
            UpdateFiguresTree();
            UpdateInterface();
        }

        private void tsmCommandLine_Click(object sender, EventArgs e)
        {
            frmCommandLine frm = new frmCommandLine(MAs);
            frm.Owner = this;
            frm.Show();
        }
    }
 
  
}
