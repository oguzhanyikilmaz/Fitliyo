export default function EgitmenDashboardPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Eğitmen Dashboard</h1>
      <p className="text-slate-600">
        Bekleyen siparişler, yaklaşan seanslar ve cüzdan özeti burada listelenecek.
      </p>
      <div className="grid gap-4 sm:grid-cols-3">
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="font-semibold text-slate-800">Cüzdan Özeti</h2>
          <p className="mt-2 text-sm text-slate-500">GET /api/app/wallet/my</p>
        </div>
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="font-semibold text-slate-800">Siparişlerim</h2>
          <p className="mt-2 text-sm text-slate-500">trainer-orders API ile doldurulacak.</p>
        </div>
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="font-semibold text-slate-800">Yaklaşan Seanslar</h2>
          <p className="mt-2 text-sm text-slate-500">Seans listesi API ile doldurulacak.</p>
        </div>
      </div>
    </div>
  );
}
