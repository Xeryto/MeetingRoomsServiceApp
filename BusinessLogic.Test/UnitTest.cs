using NUnit.Framework;
using BusinessLogic.Services;
using System;
using BusinessLogic.DAL;
using BusinessLogic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using DataAccessLayer.Models;

namespace BusinessLogic.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task UserServiceTestAsync()
        {
            IGenericRepository<User> _genericRepository = new FakeRepository<User>();
            UserService service = new(_genericRepository);
            List<User> users = new();
            User user = new()
            {
                Id = 1,
                Login = "abra",
                Password = "cadabra",
                Name = "Shapoklyak"
            };
            User user1 = new()
            {
                Id = 2,
                Login = "xeryto",
                Password = "6325",
                Name = "Vanya"
            };
            users.Add(user);
            users.Add(user1);
            (await service.Add(user)).Should().Be(user);
            (await service.Add(user1)).Should().Be(user1);
            (await service.GetAll()).Should().Equal(users);
            (await service.GetById(1)).Should().Be(user);
            service.Invoking(y => y.GetById(3)).Should()
                .Throw<Exception>().WithMessage("Entity with id 3 doesn't exist");
            (await service.GetByLogin("abra")).Should().Be(user);
            service.Invoking(y => y.GetByLogin("wronglogin")).Should().Throw<Exception>()
                .WithMessage("Entity with id 0 doesn't exist");
            (await service.GetByName("Shapoklyak")).Should().Be(user);
            service.Invoking(y => y.GetByName("wrongname")).Should().Throw<Exception>()
                .WithMessage("Entity with id 0 doesn't exist");
            service.CheckExists("abra").Should().BeTrue();
            service.CheckExists("wronglogin").Should().BeFalse();
            user.Password = "1234";
            (await service.Update(user)).Should().Be(user);
            (await service.Login("abra", "1234")).Should().Be(true);
            (await service.Login("abra", "cadabra")).Should().Be(false);
            (await service.Delete(1)).Should().Be(user);
            service.Invoking(y => y.Delete(3)).Should().Throw<Exception>()
                .WithMessage("Entity with id 3 doesn't exist");
        }

        [Test]
        public async Task MeetingRoomServiceTestAsync()
        {
            IGenericRepository<MeetingRoom> _genericRepository = new FakeRepository<MeetingRoom>();
            MeetingRoomService service = new(_genericRepository);
            List<MeetingRoom> meetingRooms = new();
            MeetingRoom meetingRoom = new()
            {
                Id = 1,
                Name = "index"
            };
            MeetingRoom meetingRoom1 = new()
            {
                Id = 2,
                Name = "pablo"
            };
            meetingRooms.Add(meetingRoom);
            meetingRooms.Add(meetingRoom1);
            (await service.Add(meetingRoom)).Should().Be(meetingRoom);
            (await service.Add(meetingRoom1)).Should().Be(meetingRoom1);
            (await service.GetAll()).Should().Equal(meetingRooms);
            (await service.GetById(1)).Should().Be(meetingRoom);
            service.Invoking(s => s.GetById(3)).Should().Throw<Exception>()
                .WithMessage("Entity with id 3 doesn't exist");
            (await service.GetByName("index")).Should().Be(meetingRoom);
            service.Invoking(y => y.GetByName("wrongname")).Should().Throw<Exception>()
                .WithMessage("Entity with id 0 doesn't exist");
            meetingRoom.Name = "changed";
            (await service.Update(meetingRoom)).Should().Be(meetingRoom);
            (await service.Delete(1)).Should().Be(meetingRoom);
            service.Invoking(y => y.Delete(3)).Should().Throw<Exception>()
                .WithMessage("Entity with id 3 doesn't exist");
        }

        [Test]
        public async Task ReservationServiceTestAsync()
        {
            DateTime from = DateTime.Now.AddHours(2);
            DateTime to = from.AddHours(2);
            DateTime from1 = from.AddDays(1);
            DateTime to1 = from1.AddHours(2);
            DateTime incorrectFrom = from.AddHours(-1);
            DateTime incorrectTo = incorrectFrom.AddHours(2);
            DateTime intervalFrom = from;
            DateTime intervalTo = intervalFrom.AddDays(3);
            FakeRepository<Reservation> _genericRepository = new();
            ReservationService service = new(_genericRepository);
            List<Reservation> reservations = new();
            List<Reservation> intervalReservations = new();
            User user = new()
            {
                Id = 1,
                Login = "abra",
                Password = "cadabra",
                Name = "Shapoklyak"
            };
            User user1 = new()
            {
                Id = 2,
                Login = "xeryto",
                Password = "6325",
                Name = "Vanya"
            };
            MeetingRoom meetingRoom = new()
            {
                Id = 1,
                Name = "index"
            };
            MeetingRoom meetingRoom1 = new()
            {
                Id = 2,
                Name = "pablo"
            };
            ReservationUpdateModel reserve = new()
            {
                Id = 1,
                UserId = user.Id,
                MeetingRoomId = meetingRoom.Id,
                From = from,
                To = to
            };
            ReservationUpdateModel reserve1 = new()
            {
                Id = 2,
                UserId = user1.Id,
                MeetingRoomId = meetingRoom1.Id,
                From = from1,
                To = to1
            };
            ReservationUpdateModel incorrectReserve = new()
            {
                Id = 3,
                UserId = user1.Id,
                MeetingRoomId = meetingRoom.Id,
                From = incorrectFrom,
                To = incorrectTo
            };
            Reservation reservation = new()
            {
                Id = reserve.Id,
                User = user,
                UserId = user.Id,
                MeetingRoom = meetingRoom,
                MeetingRoomId = meetingRoom.Id,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };
            Reservation reservation1 = new()
            {
                Id = reserve1.Id,
                UserId = user1.Id,
                MeetingRoomId = meetingRoom1.Id,
                TimeFrom = from1,
                TimeTo = to1,
                User = user1,
                MeetingRoom = meetingRoom1
            };
            reservations.Add(reservation);
            reservations.Add(reservation1);
            intervalReservations.Add(reservation1);
            (await service.Add(reserve, user, meetingRoom)).Should()
                .Be(new Tuple<bool, Reservation>(true, reservation));
            (await service.Add(reserve1, user1, meetingRoom1)).Should()
                .Be(new Tuple<bool, Reservation>(true, reservation1));
            (await service.Add(incorrectReserve, user1, meetingRoom)).Should()
                .Be(new Tuple<bool, Reservation>(false, null));
            (await _genericRepository.GetAllAsync()).Should().Equal(reservations);
            (await _genericRepository.GetByIdAsync(1)).Should().Be(reservation);
            _genericRepository.Invoking(s => s.GetByIdAsync(4)).Should().Throw<Exception>()
                .WithMessage("Entity with id 4 doesn't exist");
            reserve.From = reserve.From.AddDays(5);
            reserve.To = reserve.From.AddHours(2);
            reservation.TimeFrom = reserve.From;
            reservation.TimeTo = reserve.To;
            (await service.Update(reserve, user, meetingRoom)).Should()
                .Be(new Tuple<bool, Reservation>(true, reservation));
            _genericRepository.Query()
                .Where(x => x.MeetingRoomId == meetingRoom1.Id && x.TimeFrom < intervalTo && x.TimeTo > intervalFrom)
                .ToList().Should().Equal(intervalReservations);
            reserve1.From = reserve.From.AddHours(-1);
            reserve1.To = reserve1.From.AddHours(2);
            reserve1.MeetingRoomId = meetingRoom.Id;
            (await service.Update(reserve1, user1, meetingRoom)).Should()
                .Be(new Tuple<bool, Reservation>(false, null));
            (await service.Delete(1)).Should().Be(reservation);
            service.Invoking(y => y.Delete(4)).Should().Throw<Exception>()
                .WithMessage("Entity with id 4 doesn't exist");
        }
    }
}