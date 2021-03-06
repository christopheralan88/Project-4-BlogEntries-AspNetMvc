﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogEntries_CSharp.Models;

namespace BlogEntries_CSharp.Controllers
{
    public class BlogEntryController : Controller
    {
        private MyUser user; 
        private BlogsDb blogs = new BlogsDb(); // db or in-memory data persistence
        private UserDb users = new UserDb(); // db or in-memory data persistence

        // GET: BlogEntry
        public ActionResult Index()
        {
            if (user != null)
            {
                ViewBag.Username = user.Username;
            }
            List<BlogEntry> blogEnries = blogs.FindAllEntries();
            return View(blogEnries);
        }

        // GET:  Detail
        public ActionResult Details(string slug)
        {
            BlogEntry blogEntry = blogs.FindEntryBySlug(slug);
            return View(blogEntry);
        }

        // GET:  Sign-In
        public ActionResult SignIn()
        {
            MyUser user = new MyUser(); // TODO:  CJ make User class under Models folder
            if (TempData.ContainsKey("username") && TempData.ContainsKey("password"))
            {
                user.Username = TempData["username"].ToString();
                user.Password = TempData["password"].ToString();
                return View(user);
            }
            else
            {
                return View();
            }
        }

        // GET:  Edit
        public ActionResult Edit(string slug)
        {
            BlogEntry blogEntry = blogs.FindEntryBySlug(slug);
            return View(blogEntry);
        }

        // GET:  New
        public ActionResult New()
        {
            BlogEntry entry;
            try
            {
                entry = (BlogEntry)TempData["entry"];
                return View(entry);
            }
            catch (KeyNotFoundException)
            {
                return View(new BlogEntry());
            }
        }

        // POST:  Edit
        [HttpPost]
        public ActionResult Edit(BlogEntry entry)
        {
            bool isValidEntry = ValidateEntry(entry);
            if (!isValidEntry)
            {
                ModelState.AddModelError("Incomplete Entry", "The blog entry must have a Title, Text, and User");
                TempData.Add("entry", entry);
                return RedirectToAction("Edit");
            }

            if (ModelState.IsValid)
            {
                bool editedSuccessfully = blogs.EditBlogEntry(entry);
                if (editedSuccessfully)
                {
                    TempData["Message"] = "The entry was updated successfully!";
                }
                else
                {
                    TempData["Message"] = "Sorry, there was a problem updating your entry!";
                    TempData.Add("entry", entry);
                    return RedirectToAction("Edit");
                }
            }

            return RedirectToAction("Index");
        }

        // POST:  New
        [HttpPost]
        public ActionResult New(BlogEntry entry)
        {
            bool isValidEntry = ValidateEntry(entry);
            if (!isValidEntry)
            {
                ModelState.AddModelError("Incomplete Entry", "The blog entry must have a Title, Text, and User");
                TempData.Add("entry", entry);
                return RedirectToAction("New");
            }

            if (ModelState.IsValid)
            {
                bool addedSuccessfully = blogs.AddEntry(entry);
                if (addedSuccessfully)
                {
                    TempData["Message"] = "The entry was added successfully!";
                }
                else
                {
                    TempData["Message"] = "Sorry, there was a problem adding your entry!";
                    TempData.Add("entry", entry);
                    return RedirectToAction("New");
                }
            }

            return RedirectToAction("Index");

        }

        // POST:  SignIn
        [HttpPost]
        public ActionResult AttemptLogIn(string username, string password)
        {
            if (username != null && password != null)
            {
                user = new MyUser(username, password);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["username"] = username;
                TempData["password"] = password;
                return RedirectToAction("SignIn");
            }
        }

        private bool ValidateEntry(BlogEntry entry)
        {
            if (entry.Slug.Equals("")) return false;
            if (entry.Title.Equals("")) return false;
            if (entry.Text.Equals("")) return false;
            if (entry.User.Equals("")) return false;
            return true;
        }
    }
}