import Link from "next/link";
import { FeaturedTrainers } from "@/components/home/FeaturedTrainers";

export default function HomePage() {
  return (
    <div className="space-y-20">
      <section className="text-center">
        <h1 className="font-display text-apple-hero font-semibold tracking-tight text-apple-black md:text-5xl lg:text-6xl">
          Spor & Sağlık Koçluğu ile Hedeflerine Ulaş
        </h1>
        <p className="mx-auto mt-6 max-w-2xl text-apple-subhead font-normal text-apple-gray">
          Kişisel antrenör, diyetisyen ve spor koçlarıyla tanış. Paket seç, seanslarını planla.
        </p>
        <div className="mt-10 flex flex-wrap justify-center gap-4">
          <Link href="/trainers" className="btn-apple-primary">
            Eğitmenleri Keşfet
          </Link>
          <Link href="/register" className="btn-apple-secondary">
            Eğitmen Ol
          </Link>
        </div>
      </section>

      <FeaturedTrainers />

      <section>
        <h2 className="font-display text-apple-title font-semibold tracking-tight text-apple-black">
          Nasıl Çalışır?
        </h2>
        <ul className="mt-12 grid gap-8 sm:grid-cols-3">
          <li className="card-apple p-8">
            <span className="text-3xl font-semibold text-primary">1</span>
            <p className="mt-4 text-apple-body font-medium text-apple-black">
              Eğitmeni veya paketi seç
            </p>
            <p className="mt-2 text-apple-body text-apple-gray">
              Kategori ve hedeflerine göre filtrele.
            </p>
          </li>
          <li className="card-apple p-8">
            <span className="text-3xl font-semibold text-primary">2</span>
            <p className="mt-4 text-apple-body font-medium text-apple-black">
              Paketi satın al
            </p>
            <p className="mt-2 text-apple-body text-apple-gray">
              Güvenli ödeme ile siparişini oluştur.
            </p>
          </li>
          <li className="card-apple p-8">
            <span className="text-3xl font-semibold text-primary">3</span>
            <p className="mt-4 text-apple-body font-medium text-apple-black">
              Seansları planla
            </p>
            <p className="mt-2 text-apple-body text-apple-gray">
              Eğitmeninle birlikte tarih ve saat belirle.
            </p>
          </li>
        </ul>
      </section>
    </div>
  );
}
