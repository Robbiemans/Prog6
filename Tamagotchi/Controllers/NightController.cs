﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tamagotchi.Domein;
using Tamagotchi.Domein.Repository;
using Tamagotchi.Models;

namespace Tamagotchi.Controllers
{
    public class NightController : Controller
    {
        private IBookingRepository _bookingRepo = RepositoryLocator.Repositories.BookingRepository;

        private const string REST = "REST";
        private const string FIGHT = "FIGHT";
        private const string GAME = "GAME";
        private const string WORK = "WORK";
        private const string MISC = "MISC";
        
        public ActionResult Index()
        {
            var bookings = _bookingRepo.GetAll();

            foreach (var booking in bookings)
            {
                switch (booking.Hotelroom.Type)
                {
                    case REST:
                        booking.Tamagochi.Currency -= 10;
                        booking.Tamagochi.Health += 20;
                        if (booking.Tamagochi.Hapinness + 10 < 101)
                            booking.Tamagochi.Hapinness += 10;
                        break;
                    case GAME:
                        booking.Tamagochi.Currency -= 20;
                        booking.Tamagochi.Hapinness = 0;
                        break;
                    case WORK:
                        Random r = new Random();
                        booking.Tamagochi.Currency += r.Next(10, 60);
                        if (booking.Tamagochi.Hapinness + 20 < 101)
                            booking.Tamagochi.Hapinness += 20;
                        break;
                    case FIGHT:
                        if (booking.Hotelroom.Bookings.Count > 1)
                        {
                            Booking booking2 = booking.Hotelroom.Bookings.Where(b => b.Tamagochi.Id != booking.Tamagochi.Id).First();
                            Random ran = new Random();
                            if (ran.NextDouble() >= 0.5)
                            {
                                booking2.Tamagochi.Health -= 30;
                                booking2.Tamagochi.Currency -= 20;
                                if (booking2.Tamagochi.Health <= 0)
                                    booking2.Tamagochi.Alive = 0;
                                booking.Tamagochi.Level += 1;
                                booking.Tamagochi.Currency += 20;
                                _bookingRepo.Update(booking2);
                            }
                            else
                            {
                                booking.Tamagochi.Health -= 30;
                                booking.Tamagochi.Currency -= 20;
                                if (booking.Tamagochi.Health <= 0)
                                    booking.Tamagochi.Alive = 0;
                                booking2.Tamagochi.Level += 1;
                                booking2.Tamagochi.Currency += 20;
                                _bookingRepo.Update(booking2);
                            }
                        }
                        break;
                    case MISC:
                        booking.Tamagochi.Alive = 0;
                        break;

                }
                if (booking.Tamagochi.Hapinness >= 70)
                    booking.Tamagochi.Health -= 20;
                if (booking.Tamagochi.Health <= 0)
                    booking.Tamagochi.Alive = 0;
                booking.Tamagochi.Level += 1;
                booking.Nights -= 1;
                if (booking.Nights <= 0)
                    _bookingRepo.Remove(booking);
                else
                    _bookingRepo.Update(booking);
                RepositoryLocator.Repositories.Save();
            }


            return View(_bookingRepo.GetAll());
        }
    }
}