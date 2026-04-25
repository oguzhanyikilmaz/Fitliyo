$(function () {
    const api = {
        support: "/api/app/support-ticket",
        dispute: "/api/app/dispute",
        withdrawal: "/api/app/withdrawal-request",
        featured: "/api/app/featured-listing",
        blog: "/api/app/blog-post",
        order: "/api/app/order",
        trainer: "/api/app/trainer-profile",
        package: "/api/app/service-package",
        category: "/api/app/category",
        subscription: "/api/app/subscription",
        review: "/api/app/review",
        notification: "/api/app/notification"
    };

    const enumMaps = {
        status: { 0: "Beklemede", 1: "Onaylandı", 2: "Devam", 3: "Tamamlandı", 4: "İptal", 5: "İade" },
        paymentStatus: { 0: "Beklemede", 1: "Ödendi", 2: "Emanet", 3: "Aktarıldı", 4: "İade" },
        priority: { 0: "Düşük", 1: "Orta", 2: "Yüksek", 3: "Acil" },
        category: { 0: "Genel", 1: "Ödeme", 2: "Sipariş", 3: "Teknik", 4: "Hesap" },
        disputeType: { 0: "İade", 1: "Hizmet Yok", 2: "İptal", 3: "Diğer" },
        notificationType: { 0: "Sistem", 1: "Sipariş", 2: "Mesaj", 3: "Kampanya" },
        pageType: { 0: "Ana Sayfa", 1: "Kategori", 2: "Arama" },
        planType: { 0: "Ücretsiz", 1: "Aylık", 2: "Yıllık" }
    };

    const labelMap = {
        id: "Id",
        orderNumber: "Sipariş No",
        studentId: "Öğrenci Id",
        trainerProfileId: "Eğitmen Profil Id",
        netAmount: "Net Tutar",
        status: "Durum",
        paymentStatus: "Ödeme Durumu",
        userId: "Kullanıcı Id",
        slug: "Slug",
        city: "Şehir",
        averageRating: "Ortalama Puan",
        isActive: "Aktif",
        title: "Başlık",
        name: "Ad",
        description: "Açıklama",
        packageType: "Paket Tipi",
        price: "Fiyat",
        parentId: "Üst Kategori Id",
        sortOrder: "Sıra",
        tier: "Seviye",
        planType: "Plan Tipi",
        commissionRate: "Komisyon Oranı",
        orderId: "Sipariş Id",
        rating: "Puan",
        comment: "Yorum",
        isRead: "Okundu",
        creationTime: "Oluşturulma",
        subject: "Konu",
        category: "Kategori",
        priority: "Öncelik",
        disputeType: "Uyuşmazlık Tipi",
        amount: "Tutar",
        accountHolderName: "Hesap Sahibi",
        iban: "IBAN",
        adminNote: "Admin Notu",
        pageType: "Sayfa Tipi",
        publishedAt: "Yayın Tarihi",
        body: "İçerik",
        summary: "Özet",
        featuredImageUrl: "Kapak Görseli Url",
        notificationType: "Bildirim Tipi"
    };

    const currentModule = ($("#currentMarketplaceModule").val() || "").toString().toLowerCase();
    const modalEl = document.getElementById("marketplaceXlModal");
    const modalTitleEl = document.getElementById("marketplaceXlModalLabel");
    const modalBodyEl = document.getElementById("marketplaceXlModalBody");
    const modalSaveBtn = document.getElementById("marketplaceXlModalSaveBtn");
    const createBtn = $("#MarketplaceCreateButton");
    const marketplaceModal = modalEl && window.bootstrap ? new bootstrap.Modal(modalEl) : null;
    let table = null;

    function safe(v) { return v === null || v === undefined ? "" : String(v); }
    function notifySuccess(message) { abp.notify.success(message); }
    function notifyError(message) { abp.notify.error(message); }
    function apiGet(url) { return abp.ajax({ url, type: "GET" }); }
    function apiPost(url, data) { return abp.ajax({ url, type: "POST", data: JSON.stringify(data ?? {}) }); }
    function apiPut(url, data) { return abp.ajax({ url, type: "PUT", data: JSON.stringify(data ?? {}) }); }
    function apiDelete(url) { return abp.ajax({ url, type: "DELETE" }); }

    function turkishLabel(key) { return labelMap[key] || key; }
    function enumText(key, value) {
        const map = enumMaps[key];
        return map && Object.prototype.hasOwnProperty.call(map, value) ? map[value] : safe(value);
    }

    function showModal(options) {
        if (!marketplaceModal) return;
        modalTitleEl.textContent = options.title || "Düzenle";
        modalBodyEl.innerHTML = options.bodyHtml || "";
        if (options.onSave) {
            modalSaveBtn.classList.remove("d-none");
            modalSaveBtn.onclick = options.onSave;
        } else {
            modalSaveBtn.classList.add("d-none");
            modalSaveBtn.onclick = null;
        }
        marketplaceModal.show();
    }

    function parseValue(raw, original) {
        if (typeof original === "boolean") return String(raw).toLowerCase() === "true";
        if (typeof original === "number") return raw === "" ? 0 : Number(raw);
        return raw;
    }

    function editableFields(entity) {
        return Object.keys(entity || {}).filter(function (k) {
            return !["id", "creationTime", "creatorId", "lastModificationTime", "lastModifierId", "isDeleted", "deleterId", "deletionTime", "extraProperties", "concurrencyStamp"].includes(k);
        });
    }

    function renderEditor(entity) {
        return editableFields(entity).map(function (key) {
            const value = entity[key];
            if (typeof value === "boolean") {
                return `<div class="mb-3"><label class="form-label">${turkishLabel(key)}</label><select class="form-select js-modal-input" data-key="${key}"><option value="true" ${value ? "selected" : ""}>Evet</option><option value="false" ${!value ? "selected" : ""}>Hayır</option></select></div>`;
            }
            if (enumMaps[key]) {
                const options = Object.keys(enumMaps[key]).map(function (v) {
                    return `<option value="${v}" ${String(value) === String(v) ? "selected" : ""}>${enumMaps[key][v]}</option>`;
                }).join("");
                return `<div class="mb-3"><label class="form-label">${turkishLabel(key)}</label><select class="form-select js-modal-input" data-key="${key}">${options}</select></div>`;
            }
            const isLong = safe(value).length > 120;
            if (isLong) {
                return `<div class="mb-3"><label class="form-label">${turkishLabel(key)}</label><textarea class="form-control js-modal-input" rows="4" data-key="${key}">${safe(value)}</textarea></div>`;
            }
            return `<div class="mb-3"><label class="form-label">${turkishLabel(key)}</label><input class="form-control js-modal-input" data-key="${key}" value="${safe(value)}" /></div>`;
        }).join("");
    }

    function openUpdateModal(title, entity, saveHandler) {
        showModal({
            title,
            bodyHtml: renderEditor(entity),
            onSave: function () {
                const payload = {};
                $(modalBodyEl).find(".js-modal-input").each(function () {
                    const key = $(this).data("key");
                    payload[key] = parseValue($(this).val(), entity[key]);
                });
                saveHandler(payload);
            }
        });
    }

    function qs(input, extras) {
        const query = new URLSearchParams({
            skipCount: String(input.skipCount ?? 0),
            maxResultCount: String(input.maxResultCount ?? 10)
        });
        if (input.sorting) query.set("sorting", input.sorting);
        Object.keys(extras || {}).forEach(function (key) {
            const val = extras[key];
            if (val !== null && val !== undefined && safe(val) !== "") query.set(key, safe(val));
        });
        return query.toString();
    }

    function reload() { if (table) table.ajax.reload(); }

    function rowUpdateItem(fetchUrl, updateFn, title) {
        return {
            text: "Detay / Düzenle",
            action: function (d) {
                apiGet(fetchUrl(d)).then(function (entity) {
                    openUpdateModal(title, entity, function (payload) {
                        updateFn(d, payload).then(function () {
                            notifySuccess("Kayıt güncellendi.");
                            marketplaceModal.hide();
                            reload();
                        }).catch(function (e) {
                            notifyError(e?.message || "Güncelleme başarısız.");
                        });
                    });
                }).catch(function (e) {
                    notifyError(e?.message || "Kayıt detayı alınamadı.");
                });
            }
        };
    }

    function cfgFor(module) {
        if (module === "orders") {
            return {
                ajax: function (input) { return apiGet(`${api.order}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.order}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.order}/${d.record.id}`, payload); }, "Sipariş Güncelle"), { text: "İptal", action: function (d) { return apiPost(`${api.order}/${d.record.id}/cancel?reason=Admin`, {}); } }] } },
                    { title: "Sipariş No", data: "orderNumber" }, { title: "Öğrenci", data: "studentId" }, { title: "Eğitmen", data: "trainerProfileId" }, { title: "Tutar", data: "netAmount" }, { title: "Durum", data: "status", render: function (v) { return enumText("status", v); } }, { title: "Ödeme", data: "paymentStatus", render: function (v) { return enumText("paymentStatus", v); } }
                ]
            };
        }
        if (module === "trainers") {
            return {
                ajax: function (input) { return apiGet(`${api.trainer}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.trainer}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.trainer}/${d.record.id}`, payload); }, "Eğitmen Profili Güncelle")] } },
                    { title: "Kullanıcı", data: "userId" }, { title: "Slug", data: "slug" }, { title: "Şehir", data: "city" }, { title: "Puan", data: "averageRating" }, { title: "Aktif", data: "isActive", render: function (v) { return v ? "Evet" : "Hayır"; } }
                ]
            };
        }
        if (module === "packages") {
            return {
                ajax: function (input) { return apiGet(`${api.package}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.package}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.package}/${d.record.id}`, payload); }, "Paket Güncelle")] } },
                    { title: "Başlık", data: "title" }, { title: "Eğitmen", data: "trainerProfileId" }, { title: "Tip", data: "packageType" }, { title: "Fiyat", data: "price" }, { title: "Aktif", data: "isActive", render: function (v) { return v ? "Evet" : "Hayır"; } }
                ]
            };
        }
        if (module === "categories") {
            return {
                ajax: function (input) { return apiGet(`${api.category}?${qs(input, {})}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.category}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.category}/${d.record.id}`, payload); }, "Kategori Güncelle")] } },
                    { title: "Ad", data: "name" }, { title: "Slug", data: "slug" }, { title: "Parent", data: "parentId" }, { title: "Sıra", data: "sortOrder" }
                ]
            };
        }
        if (module === "subscriptions") {
            return {
                ajax: function (input) { return apiGet(`${api.subscription}/plans?${qs(input, {})}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.subscription}/plans/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.subscription}/${d.record.id}/plan`, payload); }, "Abonelik Planı Güncelle")] } },
                    { title: "Ad", data: "name" }, { title: "Tier", data: "tier" }, { title: "Tip", data: "planType", render: function (v) { return enumText("planType", v); } }, { title: "Fiyat", data: "price" }, { title: "Komisyon", data: "commissionRate" }
                ]
            };
        }
        if (module === "reviews") {
            return {
                ajax: function (input) { return apiGet(`${api.review}/by-trainer?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.review}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.review}/${d.record.id}`, payload); }, "Yorum Güncelle")] } },
                    { title: "Sipariş", data: "orderId" }, { title: "Öğrenci", data: "studentId" }, { title: "Eğitmen", data: "trainerProfileId" }, { title: "Puan", data: "rating" }, { title: "Yorum", data: "comment" }
                ]
            };
        }
        if (module === "notifications") {
            return {
                ajax: function (input) { return apiGet(`${api.notification}?${qs(input, {})}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.notification}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.notification}/${d.record.id}`, payload); }, "Bildirim Güncelle")] } },
                    { title: "Kullanıcı", data: "userId" }, { title: "Tip", data: "notificationType", render: function (v) { return enumText("notificationType", v); } }, { title: "Başlık", data: "title" }, { title: "Okundu", data: "isRead", render: function (v) { return v ? "Evet" : "Hayır"; } }, { title: "Tarih", data: "creationTime" }
                ]
            };
        }
        if (module === "support") {
            return {
                ajax: function (input) { return apiGet(`${api.support}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.support}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.support}/${d.record.id}`, payload); }, "Destek Talebi Güncelle")] } },
                    { title: "Konu", data: "subject" }, { title: "Kategori", data: "category", render: function (v) { return enumText("category", v); } }, { title: "Durum", data: "status", render: function (v) { return enumText("status", v); } }, { title: "Öncelik", data: "priority", render: function (v) { return enumText("priority", v); } }, { title: "Kullanıcı", data: "userId" }
                ]
            };
        }
        if (module === "disputes") {
            return {
                ajax: function (input) { return apiGet(`${api.dispute}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.dispute}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.dispute}/${d.record.id}`, payload); }, "Uyuşmazlık Güncelle")] } },
                    { title: "Tip", data: "disputeType", render: function (v) { return enumText("disputeType", v); } }, { title: "Sipariş", data: "orderId" }, { title: "Durum", data: "status", render: function (v) { return enumText("status", v); } }, { title: "Açıklama", data: "description" }
                ]
            };
        }
        if (module === "withdrawals") {
            return {
                ajax: function (input) { return apiGet(`${api.withdrawal}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.withdrawal}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.withdrawal}/${d.record.id}`, payload); }, "Para Çekme Talebi Güncelle")] } },
                    { title: "Tutar", data: "amount" }, { title: "Hesap", data: "accountHolderName" }, { title: "IBAN", data: "iban" }, { title: "Durum", data: "status", render: function (v) { return enumText("status", v); } }, { title: "Not", data: "adminNote" }
                ]
            };
        }
        if (module === "featured") {
            return {
                ajax: function (input) { return apiGet(`${api.featured}?${qs(input, { sorting: input.sorting || "sortOrder" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.featured}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.featured}/${d.record.id}`, payload); }, "Öne Çıkan Kayıt Güncelle")] } },
                    { title: "Tip", data: "pageType", render: function (v) { return enumText("pageType", v); } }, { title: "Eğitmen", data: "trainerProfileId" }, { title: "Paket", data: "servicePackageId" }, { title: "Sıra", data: "sortOrder" }, { title: "Durum", data: "isActive", render: function (v) { return v ? "Aktif" : "Pasif"; } }
                ]
            };
        }
        if (module === "blog") {
            return {
                ajax: function (input) { return apiGet(`${api.blog}?${qs(input, { sorting: input.sorting || "creationTime desc" })}`); },
                columns: [
                    { title: "İşlemler", rowAction: { items: [rowUpdateItem(function (d) { return `${api.blog}/${d.record.id}`; }, function (d, payload) { return apiPut(`${api.blog}/${d.record.id}`, payload); }, "Blog Yazısı Güncelle")] } },
                    { title: "Başlık", data: "title" }, { title: "Slug", data: "slug" }, { title: "Durum", data: "status", render: function (v) { return enumText("status", v); } }, { title: "Yayın Tarihi", data: "publishedAt" }
                ]
            };
        }
        return null;
    }

    const config = cfgFor(currentModule);
    if (!config) {
        $("#marketplaceOverview").removeClass("d-none");
        $("#marketplaceTableWrapper").addClass("d-none");
        createBtn.addClass("d-none");
        return;
    }

    $("#marketplaceOverview").addClass("d-none");
    $("#marketplaceTableWrapper").removeClass("d-none");
    createBtn.addClass("d-none");

    table = $("#MarketplaceTable").DataTable(
        abp.libs.datatables.normalizeConfiguration({
            processing: true,
            serverSide: true,
            paging: true,
            searching: false,
            scrollX: true,
            order: [],
            ajax: abp.libs.datatables.createAjax(config.ajax),
            columnDefs: config.columns
        })
    );
});
