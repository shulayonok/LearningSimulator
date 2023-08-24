using LearningSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LearningSimulator
{
    public class TestWord
    {
        public string meaning;
        public string translation;
        public byte partOfSpeech;
        public Word[] variants;
        public bool isEn;
        public TestWord(Word word, bool nextBool, IEnumerable<Word> words) 
        {
            meaning = word.Meaning;
            translation = word.Translation;
            partOfSpeech = word.PartOfSpeech;
            isEn = nextBool;
            variants = words.OrderBy(a => Guid.NewGuid()).ToArray();
        }
    }
}
