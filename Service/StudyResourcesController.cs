using DataContext;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class StudyResourcesController
    {
        private readonly AppDbContext _context;
        public StudyResourcesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<SubjectModel>> AddSubject(SubjectModel model)
        {
            try
            {
                // check subject exist
                var subjectFromDb = _context.Subjects.FirstOrDefault(x => x.Name == model.Name);
                if(subjectFromDb == null)
                {

                    await _context.Subjects.AddAsync(model);
                }
                else
                {
                    // cehck unit exist
                    var unitFromDb = _context.Units.FirstOrDefault(x => x.Name == model.Unit.First().Name && x.Subject.Name == model.Name);
                
                    if(unitFromDb == null)
                    {
                        subjectFromDb.Unit.Add(model.Unit.First());
                        _context.Subjects.Update(subjectFromDb);
                    }
                    else
                    {
                        unitFromDb.Resources.Add(model.Unit.First().Resources.First());
                        _context.Units.Update(unitFromDb);
                    }
                }


                await _context.SaveChangesAsync();

                return ServiceResponse<SubjectModel>.Success(model, "inserted to database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<SubjectModel>.Fail(null, "error occured");
            }
        }

        public async Task<ServiceResponse<List<SubjectModel>>> GetSubjects()
        {
            try
            {
                List<SubjectModel> models = await _context.Subjects.Include(x => x.Unit).ThenInclude(x => x.Resources).ToListAsync();

                return ServiceResponse<List<SubjectModel>>.Success(models, "data fetched successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<List<SubjectModel>>.Fail(null, "error occured");
            }
        }
    
        
        public async Task<ServiceResponse<SubjectModel>> RemoveSubject(int id)
        {
            try
            {
                SubjectModel? modelFromDb = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);   
                if(modelFromDb == null)
                {
                    return ServiceResponse<SubjectModel>.Fail(null, "model not found");
                }

                _context.Subjects.Remove(modelFromDb);

                await _context.SaveChangesAsync();

                return ServiceResponse<SubjectModel>.Success(modelFromDb, "subject removed from database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<SubjectModel>.Fail(null, "error occured");
            }
        }


        public async Task<ServiceResponse<UnitModel>> RemoveUnit(int id)
        {
            try
            {
                UnitModel? unitFromDb = await _context.Units.FirstOrDefaultAsync(x => x.Id == id);
                if (unitFromDb == null)
                {
                    return ServiceResponse<UnitModel>.Fail(null, "model not found");
                }

                _context.Units.Remove(unitFromDb);

                if (unitFromDb.Subject.Unit.Count == 1)
                {
                    await this.RemoveSubject(unitFromDb.SubjectId);
                }

                await _context.SaveChangesAsync();

                return ServiceResponse<UnitModel>.Success(unitFromDb, "subject removed from database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<UnitModel>.Fail(null, "error occured");
            }
        }



        public async Task<ServiceResponse<ResourcesModel>> RemoveResource(int id)
        {
            try
            {
                ResourcesModel? resourceFromDb = await _context.Resources.FirstOrDefaultAsync(x => x.Id == id);
                if (resourceFromDb == null)
                {
                    return ServiceResponse<ResourcesModel>.Fail(null, "model not found");
                }

                _context.Resources.Remove(resourceFromDb);

                if (resourceFromDb.Unit.Resources.Count == 1)
                {
                    await this.RemoveUnit(resourceFromDb.UnitId);
                }

                await _context.SaveChangesAsync();

                return ServiceResponse<ResourcesModel>.Success(resourceFromDb, "resource removed from database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ServiceResponse<ResourcesModel>.Fail(null, "error occured");
            }
        }

    }
}
