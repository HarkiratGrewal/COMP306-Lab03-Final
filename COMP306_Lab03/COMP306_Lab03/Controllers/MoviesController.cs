using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP306_Lab03.Data;
using COMP306_Lab03.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using COMP306_Lab03.Services;

namespace COMP306_Lab03.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly MoviesContext _context;
        DynamoDB dynamoService = new DynamoDB();

        public MoviesController(MoviesContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            //var all = await dynamoService.GetAllAsync();
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movies == null)
            {
                return NotFound();
            }

            return View(movies);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MoviesView model, IFormFile document)  // [Bind("Id,Name,Url,Rating,Comments")] 
        {
            if (ModelState.IsValid)
            {
                bool isUploadSuccess = false;
                if (document != null)
                {

                    Stream stream = document.OpenReadStream();
                    S3Upload s3 = new S3Upload();
                    isUploadSuccess = await s3.UploadDocument(stream, document.FileName);
                    Movies movie = new Movies
                    {
                        Name = model.Name,
                        Comments = model.Comments,
                        Rating = model.Rating,
                        //Url = "https://comp306-lab03.s3.us-east-1.amazonaws.com/" + document.FileName.Replace(" ", "+"),
                        Url = "https://s3.us-east-1.amazonaws.com/comp306-lab03/" + document.FileName.Replace(" ", "+")
                    };


                    //dynamoService.Store(movie);
                    //await stream.DisposeAsync();
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    return BadRequest("Please select file to upload");
                }

                #region UploadMessage
                /*
                if (isUploadSuccess)
                {

                    this.TempData["message"] = "Success!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error occurred" });
                }
                */
                #endregion

                //_context.Add(movies);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies.FindAsync(id);
            if (movies == null)
            {
                return NotFound();
            }
            return View(movies);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,Rating,Comments")] Movies movies)
        {
            if (id != movies.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoviesExists(movies.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movies);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movies == null)
            {
                return NotFound();
            }

            return View(movies);
        }

        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadMovie(IFormFile document)
        {
            bool isUploadSuccess = false;
            if (document != null)
            {

                Stream stream = document.OpenReadStream();
                S3Upload documentService = new S3Upload();
                isUploadSuccess = await documentService.UploadDocument(stream, document.FileName);
                Movies movie = new Movies
                {
                    //Name = document.FileName,
                    Url = "https://moviesnet.s3.us-east-2.amazonaws.com/" + document.FileName.Replace(" ", "+"),
                };


                dynamoService.Store(movie);
                await stream.DisposeAsync();

            }
            else
            {
                return BadRequest("Please select file to upload");
            }

            if (isUploadSuccess)
            {

                this.TempData["message"] = "Success!";
                return RedirectToAction("Index");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error occurred" });
            }

        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movies = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movies);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MoviesExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
