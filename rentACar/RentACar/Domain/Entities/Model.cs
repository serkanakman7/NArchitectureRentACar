using Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class Model : Entity<Guid>
    {
        public Guid BrandId { get; set; }
        public Guid FuelId { get; set; }
        public Guid TransmissId { get; set; }
        public string Name { get; set; }
        public decimal DailyPrice { get; set; }
        public string ImageUrl { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual Fuel? Fuel { get; set; }
        public virtual Transmission? Transmiss { get; set; }
        public ICollection<Car> Cars { get; set; }

        public Model()
        {
            Cars = new HashSet<Car>();
        }

        public Model(Guid id, Guid brandId, Guid fuelId, Guid transmissId, string name, decimal dailyPrice, string imageUrl) : this()
        {
            Id = id;
            BrandId = brandId;
            FuelId = fuelId;
            TransmissId = transmissId;
            Name = name;
            DailyPrice = dailyPrice;
            ImageUrl = imageUrl;
        }

    }
}
