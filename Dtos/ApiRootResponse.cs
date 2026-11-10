namespace DndForum.Dtos;

public record ApiRootResponse(string Name, string Version, IReadOnlyList<LinkDto> Links);
