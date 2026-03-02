export default function AdminDashboardPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Admin Dashboard</h1>
      <p className="text-slate-600">
        İstatistikler GET /api/app/admin/dashboard ile doldurulacak.
      </p>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Açık Destek</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">—</p>
        </div>
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Açık Uyuşmazlık</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">—</p>
        </div>
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Bekleyen Para Çekme</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">—</p>
        </div>
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam Sipariş</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">—</p>
        </div>
      </div>
    </div>
  );
}
