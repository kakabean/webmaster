using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.ide.editor.model;

namespace WebMaster.ide.editor.commands
{
    internal class DeleteNodeViewCommand :ICommand
    {
        // tobe removed node view 
        private NodeView nv = null;
        private Canvas canvas = null;
        private DiagramManager diagramManager = null;
        public DeleteNodeViewCommand(NodeView nodeview,Canvas canvas, DiagramManager diagramMgr) {
            this.nv = nodeview;
            this.canvas = canvas;
            this.diagramManager = diagramMgr;
        }
        public void execute() {
            DiagramUtil.removeNodeView(nv,canvas.Diagram, diagramManager);
            // update markers             
            canvas.updateMarkers();
            canvas.adjustSelectMarker(null);
            // update the selected element. 
            canvas.SelectedObj = canvas;
        }

        public void redo() {
            throw new NotImplementedException();
        }

        public void undo() {
            throw new NotImplementedException();
        }
    }
}
