using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.ide.editor.model;
using WebMaster.lib.engine;

namespace WebMaster.ide.editor.commands
{
    internal class DeleteConnectionCommand : ICommand
    {
        private Connection con = null;
        private Diagram diagram = null;
        private Canvas canvas = null;
        public DeleteConnectionCommand(Connection con, Canvas canvas) {
            this.con = con;
            this.canvas = canvas;
            this.diagram = canvas.Diagram;
        }
        public void execute() {
            DiagramUtil.removeConnection(con, diagram);
            // update the canvas markers 
            canvas.updateMarkers();
            canvas.adjustSelectMarker(DiagramUtil.PointNull);
            // refresh the back ground 
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
