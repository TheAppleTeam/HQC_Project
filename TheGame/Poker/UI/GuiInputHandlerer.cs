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

        public int ReadSmallBlind()
        {
            int value;
            bool parseResult = int.TryParse(this.form.textBoxSmallBlind.Text, out value);
            if (parseResult)
            {
                if (value > 100000)
                {
                    throw new InputValueException("The maximum of the Small Blind is 100 000 $");
                }
                else if (value < 250)
                {
                    throw new InputValueException("The minimum of the Small Blind is 250 $");
                }
                else
                {
                    return value;
                }
            }
            else
            {
                throw new InputValueException("The Small Blind can be only round number ");
            }
        }

        public int ReadBigBlind()
        {
            int value;
            bool parseResult = int.TryParse(this.form.textBoxBigBlind.Text, out value);
            if (parseResult)
            {
                if (value > 200000)
                {
                    throw new InputValueException("The maximum of the Small Blind is 200 000 $");
                }
                else if (value < 500)
                {
                    throw new InputValueException("The minimum of the Small Blind is 500 $");
                }
                else
                {
                    return value;
                }
            }
            else
            {
                throw new InputValueException("The Big Blind can be only round number ");
            }
           
        }
    }
}
