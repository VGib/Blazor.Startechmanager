using Blazor.Startechmanager.Shared.Constants;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions.Common;
using System.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace Blazor.Startechmanager.Server.UnitTests.StarpointsManagerTests
{
    public class CreateStarpointsTests : StarpointManagerControllerTests
    {
        [Test]
        public async Task Only_valid_starpoints_should_be_created()
        {
            var target = Create();
            target.ModelState.AddModelError("toto", "there is an error");

            var result = await target.CreateStarpoints(ThisUser.Id, new StarpointsItem());
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task CreateStarpoints_only_member_of_the_startech_should_create_starpoints_as_current_user()
        {
            SetUser(MemberDotnet);
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task CreateStarpoints_if_the_currentuser_is_not_member_of_the_startech_he_can_not_create_starpoint_for_this_startech()
        {
            SetUser(MemberDotnet);
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Agile

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            (result as ObjectResult).StatusCode.Should().IsSameOrEqualTo((int)HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task CreateStarpoints_if_the_currentuser_is_not_leader_of_the_startech_he_can_not_create_starpoint_for_this_startech_and_a_other_user_which_is_member_of_the_startech()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Java

            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            (result as ObjectResult).StatusCode.Should().IsSameOrEqualTo((int)HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task CreateStarpoints_even_startech_leader_can_not_create_starpoints_for_a_user_which_is_not_member_of_the_startech()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet

            };

            var result = await target.CreateStarpoints(User.Id, starpoint);
            (result as ObjectResult).StatusCode.Should().IsSameOrEqualTo((int)HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task CreateStarpoints_only_startech_leader_should_create_starpoint_for_another_user()
        {
            SetUser(MemberDotnet);
            UnauthorizeMember();
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet

            };

            ((Action)(() => target.CreateStarpoints(LeaderDotnet.Id, starpoint).GetAwaiter().GetResult())).Should().Throw<UnauthorizedAccessException>();
        }


        [Test]
        public async Task CreateStarpoints_save_the_created_starpoints()
        {
            SetUser(MemberDotnet);
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);

            DbContext.StarpointsItem.Any(x => x.TextJustification == newlyCreatedStarpointDescription).Should().BeTrue();
        }

        [Test]
        public async Task CreateStarpoints_only_startech_leader_can_create_starpoint_with_no_starpoints_type()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = null,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.StarpointsTypeId.Should().BeNull();
            createdStarpoint.Type.Should().BeNull();
        }

        [Test]
        public async Task when_a_startech_member_create_a_starpointsItem_with_a_starpoints_type_the_numberOfPoints_should_be_the_types_number_of_points()
        {
            SetUser(MemberDotnet);
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = null,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await  target.CreateStarpoints(ThisUser.Id, starpoint);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task a_leader_can_not_create_starpoints_from_a_non_active_type()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = NonActiveType,
                Startech = Startechs.Dotnet,
            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task a_member_can_not_create_starpoints_from_a_non_active_type()
        {
            SetUser(MemberDotnet);
            var target = Create();

            var starpoint = new StarpointsItem
            {
                Type = NonActiveType,
                Startech = Startechs.Dotnet,
            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task a_startech_leader_can_create_a_starpoint_item_with_a_type_and_a_different_starpoint_number_of_this_type()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                NumberOfPoints = 999,
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.NumberOfPoints.Should().IsSameOrEqualTo(999);
        }

        [Test]
        public async Task when_a_leader_create_a_starpoint_the_starpoint_state_is_validated()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.ValidationState.IsSameOrEqualTo(ValidationState.Validated);
        }

        [Test]
        public async Task when_a_member_create_a_starpoint_the_starpoint_state_is_in_study()
        {
            SetUser(MemberDotnet);
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.ValidationState.IsSameOrEqualTo(ValidationState.InStudy);
        }

        [Test]
        public async Task if_the_user_is_current_user_the_starpoint_should_be_created_for_him()
        {
            SetUser(MemberDotnet);
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.ApplicationUserId.IsSameOrEqualTo(MemberDotnet.Id);
        }


        [Test]
        public async Task if_another_user_is_in_parameter_the_starpoint_should_be_created_for_this_user()
        {
            SetUser(LeaderDotnet);
            AuthorizeLeader();
            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.ApplicationUserId.IsSameOrEqualTo(MemberDotnet.Id);
        }

        [Test]
        public async Task when_a_leader_create_a_validated_starpoint_the_total_amount_of_starpoint_of_the_user_shoud_be_increased()
        {
             SetUser(LeaderDotnet);
            AuthorizeLeader();

            var memberDotnet = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            memberDotnet.NumberOfPoints = 101;
            DbContext.Entry(memberDotnet).Property(x => x.NumberOfPoints).IsModified = true;
            DbContext.SaveChanges();

            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                NumberOfPoints = 112,
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(MemberDotnet.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.NumberOfPoints.IsSameOrEqualTo(101 + 112);
        }

        [Test]
        public async Task when_a_leader_create_a_inStudy_starpoint_the_total_amount_of_starpoint_should_not_change()
        {
            SetUser(MemberDotnet);

            var memberDotnet = DbContext.Users.First(x => x.Id == MemberDotnet.Id);
            memberDotnet.NumberOfPoints = 101;
            DbContext.Entry(memberDotnet).Property(x => x.NumberOfPoints).IsModified = true;
            DbContext.SaveChanges();

            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.NumberOfPoints.IsSameOrEqualTo(101);
        }

        [Test]
        public async Task created_starpoint_date_should_be_now_date()
        {
            SetUser(MemberDotnet);

            var target = Create();

            const string newlyCreatedStarpointDescription = "newly created starpoints";
            var starpoint = new StarpointsItem
            {
                Type = BlogArticle,
                Startech = Startechs.Dotnet,
                TextJustification = newlyCreatedStarpointDescription

            };

            var result = await target.CreateStarpoints(ThisUser.Id, starpoint);
            var createdStarpoint = DbContext.StarpointsItem.First(x => x.TextJustification == newlyCreatedStarpointDescription);
            createdStarpoint.Date.Should().BeCloseTo(DateTime.Now, precision : 20000);
        }
    }
}
