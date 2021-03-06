﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using Tamagotchi.Models;
using Tamagotchi.Domein.Repository;
using System;
using System.Linq;

namespace Tamagotchi.Tests
{
    [TestClass]
    public class BookingDatabaseRepositoryTest
    {
        [TestMethod]
        public void BookingDatabaseGetAll()
        {
            // Arrange
            IBookingRepository repository;
            Mock<DbSet<Booking>> set;
            Mock<TamagotchiEntities> context;
            GetContext(out repository, out set, out context);

            // Act
            List<Booking> a = repository.GetAll();

            // Assert
            Assert.AreEqual(6, a.Count);
        }

        [TestMethod]
        public void BookingDatabaseAdd()
        {
            // Arrange
            IBookingRepository repository;
            Mock<DbSet<Booking>> set;
            Mock<TamagotchiEntities> context;
            GetContext(out repository, out set, out context);

            Booking b = new Booking()
            {
                Id = 7,
                Nights = 2,
                HotelroomId = 1,
                TamagotchiId = 1
            };

            // Act
            repository.Add(b);

            // Assert
            set.Verify(x => x.Add(It.IsAny<Booking>()), Times.Once());
            List<Booking> added = set.Object.Where(x => x.Id == 7).ToList();
            Assert.AreEqual(1, added.Count);
            Assert.AreEqual(7, added[0].Id);
            Assert.AreEqual(2, added[0].Nights);
            Assert.AreEqual(1, added[0].HotelroomId);
            Assert.AreEqual(1, added[0].TamagotchiId);
        }

        [TestMethod]
        public void BookingDatabaseRemove()
        {
            // Arrange
            IBookingRepository repository;
            Mock<DbSet<Booking>> set;
            Mock<TamagotchiEntities> context;
            GetContext(out repository, out set, out context);

            Booking b = set.Object.Where(x => x.Id == 1).ToList()[0];

            // Act
            repository.Remove(b);

            // Assert
            set.Verify(x => x.Remove(It.IsAny<Booking>()), Times.Once());
            Assert.AreEqual(0, set.Object.Where(x => x.Id == 1).ToList().Count);
        }

        [TestMethod]
        public void BookingDatabaseUpdate()
        {
            // Arrange
            IBookingRepository repository;
            Mock<DbSet<Booking>> set;
            Mock<TamagotchiEntities> context;
            GetContext(out repository, out set, out context);

            Booking b = new Booking()
            {
                Id = 1,
                Nights = 2,
                HotelroomId = 2,
                TamagotchiId = 1
            };

            // Act
            repository.Add(b);

            // Assert
            set.Verify(x => x.Add(It.IsAny<Booking>()), Times.Once());
            List<Booking> updated = set.Object.Where(x => x.Id == 1).ToList();
            Assert.AreEqual(2, updated.Count);
            Assert.AreEqual(1, updated[0].Id);
            Assert.AreEqual(2, updated[0].Nights);
            Assert.AreEqual(2, updated[0].HotelroomId);
            Assert.AreEqual(1, updated[0].TamagotchiId);
        }

        [TestMethod]
        public void BookingDatabaseForceRefresh()
        {
            // Arrange
            IBookingRepository repository;
            Mock<DbSet<Booking>> set;
            Mock<TamagotchiEntities> context;
            GetContext(out repository, out set, out context);

            // Act
            bool refreshed = repository.ForceRefresh();

            // Assert
            Assert.IsTrue(refreshed);
        }

        private void GetContext(out IBookingRepository repository, out Mock<DbSet<Booking>> set, out Mock<TamagotchiEntities> context)
        {
            List<Booking> data = new List<Booking>
            {
                new Booking()
                {
                    Id = 1,
                    Nights = 2,
                    HotelroomId = 2,
                    TamagotchiId = 1
                },

                new Booking()
                {
                    Id = 2,
                    Nights = 2,
                    HotelroomId = 3,
                    TamagotchiId = 1
                },

                new Booking()
                {
                    Id = 3,
                    Nights = 2,
                    HotelroomId = 1,
                    TamagotchiId = 1
                },

                new Booking()
                {
                    Id = 4,
                    Nights = 2,
                    HotelroomId = 4,
                    TamagotchiId = 2
                },

                new Booking()
                {
                    Id = 5,
                    Nights = 2,
                    HotelroomId = 5,
                    TamagotchiId = 4
                },

                new Booking()
                {
                    Id = 6,
                    Nights = 2,
                    HotelroomId = 6,
                    TamagotchiId = 2
                }
            };

            Mock<DbSet<Booking>> mockSet = new Mock<DbSet<Booking>>();
            mockSet.As<IQueryable<Booking>>().Setup(x => x.Provider).Returns(data.AsQueryable().Provider);
            mockSet.As<IQueryable<Booking>>().Setup(x => x.Expression).Returns(data.AsQueryable().Expression);
            mockSet.As<IQueryable<Booking>>().Setup(x => x.ElementType).Returns(data.AsQueryable().ElementType);
            mockSet.As<IQueryable<Booking>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(x => x.Add(It.IsAny<Booking>())).Callback<Booking>(x => data.Add(x));
            mockSet.Setup(x => x.Remove(It.IsAny<Booking>())).Callback<Booking>(x => data.Remove(x));

            Mock<TamagotchiEntities> contextMock = new Mock<TamagotchiEntities>();
            contextMock.Setup(x => x.Bookings).Returns(mockSet.Object);

            repository = new BookingDatabaseRepository(contextMock.Object);
            set = mockSet;
            context = contextMock;
        }
    }
}
