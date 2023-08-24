using LearningSimulator.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningSimulator
{
    public class UsersAsyncRepository
    {
        private readonly SQLiteAsyncConnection database;

        public UsersAsyncRepository(string databasePath)
        {
            var options = new SQLiteConnectionString(databasePath, true, "EnjoyLearningEnglish!", postKeyAction: c =>
            {
                c.Execute("PRAGMA cipher_compatibility = 3");
            });
            database = new SQLiteAsyncConnection(options);
            //database = new SQLiteAsyncConnection(databasePath);
            database.CreateTablesAsync().Wait();   
        }

        #region User entity
        public async Task CreateUserTable() => await database.CreateTableAsync<User>();
        public async Task<List<User>> GetUsersAsync() => await database.Table<User>().ToListAsync();
        public async Task<User> GetUserAsync(int id) => await database.GetAsync<User>(id);

        public async Task<int> DeleteUserAsync(User item)
        {
            // Удаляем все записи в промежуточной таблице с этим юзером, слова, которые есть только у него
            List<UserWord> allIntermediates = await GetIntermediatesAsync();
            List<UserWord> userIntermediates = allIntermediates.Where(i => i.UserID == item.ID).ToList();
            List<UserWord> otherIntermediates = allIntermediates.Where(i => i.UserID != item.ID).ToList();
            List<int> userWords = new List<int>();
            List<int> otherWords = new List<int>();

            otherIntermediates.ForEach(intermediate =>
            {
                otherWords.Add(intermediate.WordID);
            });

            userIntermediates.ForEach(async intermediate => 
            {
                if (!otherWords.Contains(intermediate.WordID)) await database.DeleteAsync(await GetWordAsync(intermediate.WordID));
                await database.DeleteAsync(intermediate);
            });

            return await database.DeleteAsync(item);
        }
        public async Task<int> SaveUserAsync(User item)
        {
            if (item.ID != 0)
            {
                await database.UpdateAsync(item);
                return item.ID;
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }
        public async Task<User> FindUserAsync(string username)
        {
            return await database.FindAsync<User>(u => u.Username == username);
        }
        #endregion

        #region Word entity
        public async Task CreateWordTable() => await database.CreateTableAsync<Word>();
        public async Task<List<Word>> GetWordsAsync() => await database.Table<Word>().ToListAsync();
        public async Task<List<Word>> GetUserWordsAsync(User user)
        {
            List<UserWord> allIntermediates = await GetIntermediatesAsync();
            List<UserWord> userIntermediates = allIntermediates.Where(i => i.UserID == user.ID).ToList();
            List<int> userWords = new List<int>();
            userIntermediates.ForEach(intermediate => { userWords.Add(intermediate.WordID); });
            return await database.Table<Word>().Where(w => userWords.Contains(w.ID)).ToListAsync();
        }
        public async Task<Word> GetWordAsync(int id) => await database.GetAsync<Word>(id);
        public async Task<int> DeleteWordAsync(User user, Word item)
        {
            Word toFind = await FindWordAsync(item.Meaning);
            if (toFind != null)
            {
                List<UserWord> allIntermediates = await GetIntermediatesAsync();
                bool isContained = allIntermediates.Where(i => i.UserID != user.ID && i.WordID == item.ID).Any();
                if (!isContained) await database.DeleteAsync(item);
                UserWord userWord = await FindIntermediateAsync(user.ID, item.ID);
                return await database.DeleteAsync(userWord);
            }
            else return 0;
        }

        public async Task<int> SaveWordAsync(User user, Word item)
        {
            if (item.ID != 0)
            {
                await database.UpdateAsync(item);
                return item.ID;
            }
            else
            {
                Word word = await FindWordAsync(item.Meaning);
                if (word != null)
                {
                    // Добавляем запись только в промежуточную таблицу
                    UserWord userWord = await FindIntermediateAsync(user.ID, word.ID);
                    if (userWord != null) return 0;
                    else return await database.InsertAsync(new UserWord() { UserID = user.ID, WordID = word.ID });
                }
                else
                {
                    // Добавляем новое слово и запись в промежуточную таблицу
                    int res = await database.InsertAsync(item);
                    res += await database.InsertAsync(new UserWord() { UserID = user.ID, WordID = item.ID });
                    return res;
                }
            }

        }

        public async Task<Word> FindWordAsync(string meaning)
        {
            return await database.FindAsync<Word>(w => w.Meaning == meaning);
        }
        #endregion

        #region Intermediate table
        public async Task CreateIntermediateTable() => await database.CreateTableAsync<UserWord>();
        public async Task<List<UserWord>> GetIntermediatesAsync() => await database.Table<UserWord>().ToListAsync();
        public async Task<UserWord> FindIntermediateAsync(int userId, int wordId)
        {
            return await database.FindAsync<UserWord>(i => (i.UserID == userId && i.WordID == wordId));
        }
        #endregion

        #region Phrasal verb entity
        public async Task CreatePVTable() => await database.CreateTableAsync<PhrasalVerb>();
        public async Task<List<PhrasalVerb>> GetPVAsync() => await database.Table<PhrasalVerb>().ToListAsync();
        public async Task<List<PhrasalVerb>> GetUserPVAsync(User user)
        {
            List<UserPhrasalVerb> allIntermediates = await GetIntermediatesPVAsync();
            List<UserPhrasalVerb> userIntermediates = allIntermediates.Where(i => i.UserID == user.ID).ToList();
            List<int> userWords = new List<int>();
            userIntermediates.ForEach(intermediate => { userWords.Add(intermediate.PhrasalVerbID); });
            return await database.Table<PhrasalVerb>().Where(w => userWords.Contains(w.ID)).ToListAsync();
        }
        public async Task<PhrasalVerb> GetPVAsync(int id) => await database.GetAsync<PhrasalVerb>(id);
        public async Task<int> DeletePVAsync(User user, PhrasalVerb item)
        {
            PhrasalVerb toFind = await FindPVAsync(item.Meaning);
            if (toFind != null)
            {
                List<UserPhrasalVerb> allIntermediates = await GetIntermediatesPVAsync();
                bool isContained = allIntermediates.Where(i => i.UserID != user.ID && i.PhrasalVerbID == item.ID).Any();
                if (!isContained) await database.DeleteAsync(item);
                UserPhrasalVerb userWord = await FindIntermediatePVAsync(user.ID, item.ID);
                return await database.DeleteAsync(userWord);
            }
            else return 0;
        }

        public async Task<int> SavePVAsync(User user, PhrasalVerb item)
        {
            if (item.ID != 0)
            {
                await database.UpdateAsync(item);
                return item.ID;
            }
            else
            {
                PhrasalVerb pv = await FindPVAsync(item.Meaning);
                if (pv != null)
                {
                    // Добавляем запись только в промежуточную таблицу
                    UserPhrasalVerb userPV = await FindIntermediatePVAsync(user.ID, pv.ID);
                    if (userPV != null) return 0;
                    else return await database.InsertAsync(new UserPhrasalVerb() { UserID = user.ID, PhrasalVerbID = pv.ID });
                }
                else
                {
                    // Добавляем новое слово и запись в промежуточную таблицу
                    int res = await database.InsertAsync(item);
                    res += await database.InsertAsync(new UserPhrasalVerb() { UserID = user.ID, PhrasalVerbID = item.ID });
                    return res;
                }
            }

        }

        public async Task<PhrasalVerb> FindPVAsync(string meaning)
        {
            return await database.FindAsync<PhrasalVerb>(w => w.Meaning == meaning);
        }
        #endregion

        #region Intermediate (phrasal verbs) table
        public async Task CreateIntermediatePVTable() => await database.CreateTableAsync<UserPhrasalVerb>();
        public async Task<List<UserPhrasalVerb>> GetIntermediatesPVAsync() => await database.Table<UserPhrasalVerb>().ToListAsync();
        public async Task<UserPhrasalVerb> FindIntermediatePVAsync(int userId, int pvId)
        {
            return await database.FindAsync<UserPhrasalVerb>(i => (i.UserID == userId && i.PhrasalVerbID == pvId));
        }
        #endregion
    }
}
