namespace ApiBureau.Bullhorn.Api.Dtos;

public class CandidateDto : CandidateBaseDto
{
    public List<string>? Occupation { get; set; }
    public EntityChildrenDto? Sendouts { get; set; }
    public EntityChildrenDto? Placements { get; set; }
    public EntityChildrenDto? Interviews { get; set; }
    public EntityChildrenDto? FileAttachments { get; set; }

    public CandidateDto() { }

    public bool ShouldSerializeOccupation() => Occupation == null || Occupation.Count != 0;
    public bool ShouldSerializeSendouts() => Sendouts == null;
    public bool ShouldSerializePlacements() => Placements == null;
    public bool ShouldSerializeInterviews() => Interviews == null;
}
