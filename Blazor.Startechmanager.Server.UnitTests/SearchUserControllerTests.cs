using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class SearchUserControllerTests : BaseTests<SearchUserController>
    {
        public void PopulateDatas()
        {
            for(int n = 0; n < 15; ++n)
            {
                DbContext.Users.Add(new ApplicationUser
                {
                    Id = 100 + n,
                    UserName = $"test_abc_{n}"
                });
            }

            DbContext.Users.Add(new ApplicationUser
            {
                Id = 1,
                UserName = "totoabctiti"
            });

            DbContext.Users.Add(new ApplicationUser
            {
                Id = 2,
                UserName = "abctutu"
            });
            DbContext.Users.Add(new ApplicationUser
            {
                Id = 3,
                UserName = "zozototoabc"
            });
            DbContext.Users.Add(new ApplicationUser
            {
                Id = 3,
                UserName = "youpladefghil"
            });

            DbContext.SaveChanges();
        }

        public async Task should_not_return_a_user_if_their_is_less_than_3_characters()
        {
            var target = Create();
            var result = await target.Search("ab");
            result.Should().BeEmpty();
        }

        public async Task should_return_the_user_who_match_the_like_pattern()
        {
            var target = Create();
            var result = await target.Search("abc");
            result.Select(x => x.UserName).Should().BeEquivalentTo("totoabctiti", "abctutu", "zozototoabc");
        }

        public async Task should_return_a_least_10_values()
        {
            var target = Create();
            var result = await target.Search("test");
            result.Should().HaveCount(10);
        }
    }
}
