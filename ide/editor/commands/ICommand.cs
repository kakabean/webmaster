using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.ide.editor.commands
{
    public interface ICommand
    {
        void execute();
        void redo();
        void undo();
    }
}
