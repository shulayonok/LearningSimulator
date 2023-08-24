using LearningSimulator.Models;
using Xamarin.Forms;

namespace LearningSimulator
{
    internal class PartOfSpeechSelector : DataTemplateSelector
    {
        public DataTemplate Noun { get; set; }
        public DataTemplate Verb { get; set; }
        public DataTemplate Adjective { get; set; }
        public DataTemplate Adverb { get; set; }
        public DataTemplate Participle { get; set; }
        public DataTemplate Preposition { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            switch (((Word)item).PartOfSpeech)
            {
                case 1:
                    return Noun;
                case 2:
                    return Verb;
                case 3:
                    return Adjective;
                case 4:
                    return Adverb;
                case 5:
                    return Participle;
                default:
                    return Preposition;
            }
        }
    }
}
