using LearningSimulator.Models;
using LearningSimulator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningSimulator
{
    public class TestHelper
    {
        Random random = new Random();
        byte quantity = 30;
        public bool NextBool(int truePercentage = 50)
        {
            return random.NextDouble() < truePercentage / 100.0;
        }
        public async Task<List<TestWord>> GetWords()
        {
            List<Word> allWords = await App.Database.GetUserWordsAsync(LoginPage.user);
            List<TestWord> words = new List<TestWord>();
            List<Word> nouns = allWords.Where(w => w.PartOfSpeech == 1).OrderBy(w => Guid.NewGuid()).ToList();
            List<Word> verbs = allWords.Where(w => w.PartOfSpeech == 2).OrderBy(w => Guid.NewGuid()).ToList();
            List<Word> adjectives = allWords.Where(w => w.PartOfSpeech == 3).OrderBy(w => Guid.NewGuid()).ToList();
            List<Word> adverbs = allWords.Where(w => w.PartOfSpeech == 4).OrderBy(w => Guid.NewGuid()).ToList();
            List<Word> participles = allWords.Where(w => w.PartOfSpeech == 5).OrderBy(w => Guid.NewGuid()).ToList();
            List<Word> prepositions = allWords.Where(w => w.PartOfSpeech == 6).OrderBy(w => Guid.NewGuid()).ToList();
            byte count = 0;
            while (count != quantity)
            {
                if (NextBool(40) && nouns.Count() > 5)
                {
                    words.Add(new TestWord(nouns.First(), NextBool(), nouns.Take(5)));
                    nouns = nouns.Skip(5).ToList();
                    count++;
                }
                else if (NextBool(25) && verbs.Count() > 5)
                {
                    words.Add(new TestWord(verbs.First(), NextBool(), verbs.Take(5)));
                    verbs = verbs.Skip(5).ToList();
                    count++;
                }
                else if (NextBool(20) && adjectives.Count() > 5)
                {
                    words.Add(new TestWord(adjectives.First(), NextBool(), adjectives.Take(5)));
                    adjectives = adjectives.Skip(5).ToList();
                    count++;
                }
                else if (NextBool(9) && adverbs.Count() > 5)
                {
                    words.Add(new TestWord(adverbs.First(), NextBool(), adverbs.Take(5)));
                    adverbs = adverbs.Skip(5).ToList();  
                    count++;
                }
                else if (NextBool(3) && participles.Count() > 5)
                {
                    words.Add(new TestWord(participles.First(), NextBool(), participles.Take(5)));
                    participles = participles.Skip(5).ToList();
                    count++;
                }
                else if (NextBool(3) && prepositions.Count() > 5)
                {
                    words.Add(new TestWord(prepositions.First(), NextBool(), prepositions.Take(5)));
                    prepositions = prepositions.Skip(5).ToList();    
                    count++;
                }
            }
            return words;
        }
    }
}
