using prjSlotMachine.Controller;
using prjSlotMachine.Model;
using prjSlotMachine.View;

namespace prjSlotMachine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SlotMachineModel model = new SlotMachineModel();
            SlotMachineView view = new SlotMachineView();
            SlotMachineController controller = new SlotMachineController(model, view);

            controller.Spin();
        }
    }
}
