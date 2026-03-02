"use client";

/**
 * Liste sayfalarında tutarlı loading / error / boş durum gösterimi (3.4 Frontend Teknik Eksikler).
 */

interface LoadingStateProps {
  message?: string;
  className?: string;
}

export function LoadingState({ message = "Yükleniyor...", className = "" }: LoadingStateProps) {
  return (
    <div className={`flex min-h-[40vh] items-center justify-center ${className}`}>
      <p className="text-apple-body text-apple-gray">{message}</p>
    </div>
  );
}

interface ErrorStateProps {
  message: string;
  title?: string;
  onRetry?: () => void;
  className?: string;
}

export function ErrorState({ message, title, onRetry, className = "" }: ErrorStateProps) {
  return (
    <div className={`space-y-4 ${className}`}>
      {title && (
        <h1 className="font-display text-apple-title font-semibold text-apple-black">{title}</h1>
      )}
      <div className="card-apple border border-amber-200 bg-amber-50 p-4 text-amber-800">
        {message}
      </div>
      {onRetry && (
        <button
          type="button"
          onClick={onRetry}
          className="rounded-lg bg-apple-black px-4 py-2 text-sm font-medium text-white hover:opacity-90"
        >
          Tekrar dene
        </button>
      )}
    </div>
  );
}

interface EmptyStateProps {
  message?: string;
  className?: string;
  children?: React.ReactNode;
}

export function EmptyState({
  message = "Henüz kayıt yok.",
  className = "",
  children,
}: EmptyStateProps) {
  return (
    <div
      className={`rounded-xl border border-dashed border-apple-grayLighter bg-slate-50 p-8 text-center text-apple-body text-apple-gray ${className}`}
    >
      {message}
      {children}
    </div>
  );
}
