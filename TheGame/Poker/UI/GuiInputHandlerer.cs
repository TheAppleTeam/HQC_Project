namespace Poker.UI
{
    using Exception;

    public class GuiInputHandlerer : IInputHandlerer
    {
        private GameForm form;

        public GuiInputHandlerer(GameForm form)
        {
            this.form = form;
        }

        public int ReadRaise()
        {
            int value;
            bool parseResult = int.TryParse(this.form.textBoxRaise.Text, out value);
            if (parseResult)
            {
                return value;
            }
            throw new InputValueException("This is a number only field");
        }

        public int ReadChipsToAdd()
        {
            int value;
            bool parseResult = int.TryParse(this.form.textBoxAddChips.Text, out value);
            if (parseResult)
            {
                return value;
            }

            return 0;
        }
    }
}
