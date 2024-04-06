using Bamboozlers.Classes.AppDbContext;
using Bamboozlers.Classes.Func;

namespace Bamboozlers.Classes.Data;

public record ChatContext(
    bool IsMod,
    AsyncConsumer<string?> SetLastEdit,
    AsyncConsumer<Message> OnDelete,
    AsyncConsumer<Message> OnEditStart,
    AsyncConsumer<bool> OnEditEnd,
    AsyncConsumer<Message> OnPin);