using Bamboozlers.Classes.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable.Moq;

namespace Tests.Provider;

public class MockDatabaseProvider
{
    private readonly Mock<IDbContextFactory<AppDbContext>> _mockDbContextFactory;

    public MockDatabaseProvider(TestContextBase ctx)
    {
        _mockDbContextFactory = new Mock<IDbContextFactory<AppDbContext>>();

        var options = new DbContextOptions<AppDbContext>();
        var mockDbContext = new Mock<AppDbContext>(options);
        SetupMockDbContext(mockDbContext);

        _mockDbContextFactory.Setup(x => x.CreateDbContext()).Returns(mockDbContext.Object);
        _mockDbContextFactory.Setup(x => x.CreateDbContextAsync(default)).ReturnsAsync(mockDbContext.Object);

        ctx.Services.AddSingleton(GetDbContextFactory());
    }

    public IDbContextFactory<AppDbContext> GetDbContextFactory()
    {
        return _mockDbContextFactory.Object;
    }

    private void SetupMockDbContext(Mock<AppDbContext> mockDbContext)
    {
        var mockUsers = SetupMockUsers();
        var mockChats = SetupMockChats(mockUsers.Object);
        var mockMessages = SetupMockMessages(mockUsers.Object, mockChats.Object);
        var mockBlocks = SetupMockBlocks(mockUsers.Object);
        var mockFriendRequests = SetupMockFriendRequests(mockUsers.Object);
        var mockFriendships = SetupMockFriendships(mockUsers.Object);
        var mockGroupInvites = SetupMockDbSet(new List<GroupInvite>());

        mockDbContext.Setup(x => x.Users).Returns(mockUsers.Object);
        mockDbContext.Setup(x => x.Chats).Returns(mockChats.Object);
        mockDbContext.Setup(x => x.Messages).Returns(mockMessages.Object);
        mockDbContext.Setup(x => x.BlockList).Returns(mockBlocks.Object);
        mockDbContext.Setup(x => x.FriendRequests).Returns(mockFriendRequests.Object);
        mockDbContext.Setup(x => x.FriendShips).Returns(mockFriendships.Object);
        mockDbContext.Setup(x => x.GroupInvites).Returns(mockGroupInvites.Object);
    }

    private Mock<DbSet<User>> SetupMockUsers()
    {
        return SetupMockDbSet(new List<User>
        {
            new()
            {
                Id = 0,
                AccessFailedCount = 0,
                Chats = [],
                ModeratedChats = [],
                OwnedChats = [],
                UserName = "TestUser0",
                Email = "test_user0@gmail.com",
                EmailConfirmed = true
            },
            new()
            {
                Id = 1,
                AccessFailedCount = 0,
                Chats = [],
                ModeratedChats = [],
                OwnedChats = [],
                UserName = "TestUser1",
                Email = "test_user1@gmail.com",
                EmailConfirmed = true
            },
            new()
            {
                Id = 2,
                AccessFailedCount = 0,
                Chats = [],
                ModeratedChats = [],
                OwnedChats = [],
                UserName = "TestUser2",
                Email = "test_user2@gmail.com",
                EmailConfirmed = true
            }
        });
    }

    private Mock<DbSet<Chat>> SetupMockChats(IQueryable<User> users)
    {
        var user1 = users.First();
        var user2 = users.Skip(1).First();
        var user3 = users.Last();

        var dm = new Chat
        {
            ID = 1,
            Users = new List<User> { user1, user2 },
            Messages = []
        };
        
        var groupChat = new GroupChat
        {
            ID = 2,
            Name = "TestGroupChat",
            Owner = user1,
            OwnerID = user1.Id,
            Moderators = new List<User> { user2 },
            Users = new List<User> { user1, user2, user3 },
            Messages = []
        };
        
        user1.Chats.Add(dm);
        user1.Chats.Add(groupChat);
        user1.OwnedChats.Add(groupChat);
        user2.Chats.Add(dm);
        user2.Chats.Add(groupChat);
        user2.ModeratedChats.Add(groupChat);
        user3.Chats.Add(groupChat);
        
        return SetupMockDbSet(new List<Chat> { dm, groupChat });
    }

    private Mock<DbSet<Message>> SetupMockMessages(IQueryable<User> users, IQueryable<Chat> chats)
    {
        var user1 = users.First();
        var user2 = users.Skip(1).First();
        var dm = chats.First();
        
        var messages = new List<Message>
        {
            new()
            {
                ID = 1,
                Chat = dm,
                ChatID = dm.ID,
                Sender = user1,
                SenderID = user1.Id,
                Content = "Hello World!",
                SentAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(5))
            },
            new()
            {
                ID = 2,
                Chat = dm,
                ChatID = dm.ID,
                Sender = user2,
                SenderID = user2.Id,
                Content = "Hi!",
                SentAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(4))
            },
            new()
            {
                ID = 3,
                Chat = dm,
                ChatID = dm.ID,
                Sender = user2,
                SenderID = user2.Id,
                Content = "How are you?",
                SentAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(3.5))
            },
            new()
            {
                ID = 4,
                Chat = dm,
                ChatID = dm.ID,
                Sender = user1,
                SenderID = user1.Id,
                Content = "I'm good, you?",
                SentAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(3))
            },
            new()
            {
                ID = 5,
                Chat = dm,
                ChatID = dm.ID,
                Sender = user2,
                SenderID = user2.Id,
                Content = "I'm good too!",
                SentAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(2))
            }
        };
        
        for (var i = 6; i < 100; i++)
        {
            messages.Add(new Message
            {
                ID = i,
                Chat = dm,
                ChatID = dm.ID,
                Sender = user1,
                SenderID = user1.Id,
                Content = "Test Message " + i,
                SentAt = DateTime.Now.Subtract(TimeSpan.FromSeconds(101 - i))
            });
        }
        
        dm.Messages = messages;
        
        return SetupMockDbSet(messages);
    }

    private Mock<DbSet<Block>> SetupMockBlocks(IQueryable<User> users)
    {
        var user2 = users.Skip(1).First();
        var user3 = users.Last();

        return SetupMockDbSet(new List<Block>
        {
            new()
            {
                Blocked = user2,
                BlockedID = user2.Id,
                Blocker = user3,
                BlockerID = user3.Id
            }
        });
    }

    private Mock<DbSet<FriendRequest>> SetupMockFriendRequests(IQueryable<User> users)
    {
        var user1 = users.First();
        var user3 = users.Last();

        return SetupMockDbSet(new List<FriendRequest>
        {
            new()
            {
                Sender = user1,
                SenderID = user1.Id,
                Receiver = user3,
                ReceiverID = user3.Id
            }
        });
    }

    private Mock<DbSet<Friendship>> SetupMockFriendships(IQueryable<User> users)
    {
        var user1 = users.First();
        var user2 = users.Skip(1).First();

        return SetupMockDbSet(new List<Friendship>
        {
            new()
            {
                User1 = user1,
                User1ID = user1.Id,
                User2 = user2,
                User2ID = user2.Id
            }
        });
    }

    private Mock<DbSet<T>> SetupMockDbSet<T>(IEnumerable<T> data) where T : class
    {
        return data.AsQueryable().BuildMockDbSet();
    }
}