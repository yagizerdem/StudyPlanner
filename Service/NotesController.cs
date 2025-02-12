using DataContext;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Service
{
    public class NotesController
    {
        private readonly AppDbContext _context;
        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<NotesModel>> AddNote(NotesModel newNote)
        {
            try
            {
                await _context.Notes.AddAsync(newNote);
                _context.SaveChanges();

                return ServiceResponse<NotesModel>.Success(newNote, "new note added");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<NotesModel>.Fail(null, "error occured");
            }
        }

        public async Task<ServiceResponse<List<NotesModel>>> GetNotes()
        {
            try
            {
                var notes =  await _context.Notes.ToListAsync<NotesModel>();
                return ServiceResponse<List<NotesModel>>.Success(notes, "notes fetched successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<List<NotesModel>>.Fail(null, "error occured");
            }
        }


        public async Task<ServiceResponse<NotesModel>> Delete(int id)
        {

            try
            {
                var noteFromDb = await _context.Notes.FirstOrDefaultAsync<NotesModel>(x  => x.Id == id);
                if(noteFromDb == null)
                {
                    return ServiceResponse<NotesModel>.Fail(null, "not not  found in db");
                }

                _context.Notes.Remove(noteFromDb);
                await _context.SaveChangesAsync(); 

                return ServiceResponse<NotesModel>.Success(noteFromDb, "note deleted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<NotesModel>.Fail(null, "error occured");
            }
        }


        public async Task<ServiceResponse<NotesModel>> AddToStarred(int id)
        {

            try
            {
                var noteFromDb = await _context.Notes.FirstOrDefaultAsync<NotesModel>(x => x.Id == id);
                if (noteFromDb == null)
                {
                    return ServiceResponse<NotesModel>.Fail(null, "not not  found in db");
                }
                noteFromDb.Starred = true;

                _context.Notes.Update(noteFromDb);
                await _context.SaveChangesAsync();

                return ServiceResponse<NotesModel>.Success(noteFromDb, "note deleted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<NotesModel>.Fail(null, "error occured");
            }
        }

        public async Task<ServiceResponse<NotesModel>> RemoveFromStarred(int id)
        {
            try
            {
                var noteFromDb = await _context.Notes.FirstOrDefaultAsync<NotesModel>(x => x.Id == id);
                if (noteFromDb == null)
                {
                    return ServiceResponse<NotesModel>.Fail(null, "not not  found in db");
                }
                noteFromDb.Starred = false;

                _context.Notes.Update(noteFromDb);
                await _context.SaveChangesAsync();

                return ServiceResponse<NotesModel>.Success(noteFromDb, "note deleted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<NotesModel>.Fail(null, "error occured");
            }
        }

    }
}
