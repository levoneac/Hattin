namespace HattinEngineLibrary.Types
{
    public class AttackInformation
    {
        public required ColorCount AttackTotals { get; set; }
        public required List<AttackProjection> Data { get; set; }
    }
}