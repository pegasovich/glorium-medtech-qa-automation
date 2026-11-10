namespace Glorium.MedTech.MockServer;

public static class HtmlTemplates
{
    public static string GetLoginPageHtml() => """
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8"/>
      <title>Glorium MedTech Portal - Login</title>
      <style>
        body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; background: #f0f4f8; display:flex; justify-content:center; align-items:center; height:100vh; margin:0;}
        .card { background: white; padding: 2.5rem; border-radius: 8px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); width: 380px; }
        h2 { margin-top:0; color: #1e3a8a; }
        label { display: block; margin: 12px 0 4px; font-weight: 600; font-size: 0.875rem; color: #374151; }
        input[type="text"], input[type="password"] { width: 100%; padding: 10px; border: 1px solid #d1d5db; border-radius: 4px; box-sizing: border-box; }
        button { width: 100%; margin-top: 20px; padding: 10px; background: #2563eb; color: white; border: none; border-radius: 4px; font-weight: 600; cursor: pointer; }
        button:hover { background: #1d4ed8; }
        .error { color: #dc2626; font-size: 0.875rem; margin-top: 8px; display: none; }
      </style>
    </head>
    <body>
      <div class="card">
        <h2 data-testid="portal-header">MedTech Portal Login</h2>
        <p style="color:#6b7280; font-size:0.875rem;">Home Medical Equipment Management</p>
        <form id="loginForm" onsubmit="event.preventDefault(); handleLogin();">
          <label for="username">Username / Email</label>
          <input id="username" data-testid="input-username" type="text" placeholder="dispatcher@gloriummed.com" required/>
          
          <label for="password">Password</label>
          <input id="password" data-testid="input-password" type="password" required/>
          
          <div id="error-msg" data-testid="login-error-message" class="error">Invalid clinician credentials</div>
          
          <button type="submit" data-testid="btn-login">Sign In</button>
        </form>
      </div>
      <script>
        function handleLogin() {
          const p = document.getElementById('password').value;
          if (p === 'GloriumPass123!') {
            localStorage.setItem('medtech_token', 'jwt_sample_token');
            window.location.href = '/equipment';
          } else {
            document.getElementById('error-msg').style.display = 'block';
          }
        }
      </script>
    </body>
    </html>
    """;

    public static string GetEquipmentPageHtml() => """
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8"/>
      <title>Equipment Catalog - Glorium MedTech</title>
      <style>
        body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; background: #f8fafc; margin: 0; padding: 24px; }
        .header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #e2e8f0; padding-bottom: 16px; }
        h1 { color: #0f172a; margin: 0; }
        .btn-order { background: #059669; color: white; padding: 10px 16px; border-radius: 6px; text-decoration: none; font-weight: 600; }
        .filter-bar { margin: 20px 0; display: flex; gap: 12px; }
        select, input { padding: 8px 12px; border: 1px solid #cbd5e1; border-radius: 4px; }
        .catalog-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 16px; }
        .card { background: white; border: 1px solid #e2e8f0; border-radius: 8px; padding: 16px; box-shadow: 0 1px 3px rgba(0,0,0,0.05); }
        .category-badge { background: #dbeafe; color: #1e40af; font-size: 0.75rem; padding: 2px 8px; border-radius: 12px; font-weight: 600; }
        .price { font-size: 1.25rem; font-weight: bold; color: #0f172a; margin: 12px 0; }
      </style>
    </head>
    <body>
      <div class="header">
        <h1 data-testid="catalog-title">Medical Equipment Catalog</h1>
        <div>
          <a href="/orders/new" data-testid="btn-create-order" class="btn-order">+ New Equipment Order</a>
        </div>
      </div>
      <div class="filter-bar">
        <select id="categoryFilter" data-testid="select-category" onchange="filterCategory()">
          <option value="All">All Categories</option>
          <option value="Respiratory">Respiratory</option>
          <option value="Mobility">Mobility</option>
          <option value="Monitoring">Monitoring</option>
        </select>
        <span id="equipment-count" data-testid="equipment-count" style="align-self: center; color: #64748b; font-size: 0.9rem;">Loading...</span>
      </div>
      <div id="grid" class="catalog-grid" data-testid="equipment-grid"></div>
      <script>
        let allEquipment = [];
        async function load() {
          const res = await fetch('/api/v1/equipment');
          allEquipment = await res.json();
          render(allEquipment);
        }
        function render(items) {
          const grid = document.getElementById('grid');
          grid.innerHTML = items.map(item => `
            <div class="card" data-testid="equipment-card-${item.id}">
              <span class="category-badge">${item.category}</span>
              <h3 style="margin: 8px 0;" data-testid="equipment-name">${item.name}</h3>
              <p style="color:#64748b; font-size:0.85rem;">Serial: <code>${item.serialNumber}</code></p>
              <div class="price">$${item.dailyRate.toFixed(2)} <span style="font-size:0.8rem; font-weight:normal; color:#64748b;">/ day</span></div>
              <span style="color: #059669; font-weight:600; font-size:0.85rem;" data-testid="equipment-status">● ${item.status}</span>
            </div>
          `).join('');
          document.getElementById('equipment-count').innerText = `Showing ${items.length} items`;
        }
        function filterCategory() {
          const cat = document.getElementById('categoryFilter').value;
          if (cat === 'All') render(allEquipment);
          else render(allEquipment.filter(e => e.category === cat));
        }
        load();
      </script>
    </body>
    </html>
    """;

    public static string GetCreateOrderPageHtml() => """
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8"/>
      <title>Schedule Equipment Delivery - Glorium MedTech</title>
      <style>
        body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; background: #f8fafc; padding: 32px; margin: 0; }
        .container { max-width: 600px; margin: 0 auto; background: white; padding: 28px; border-radius: 8px; border: 1px solid #e2e8f0; }
        h1 { margin-top: 0; color: #0f172a; font-size: 1.5rem; }
        label { display: block; margin: 16px 0 6px; font-weight: 600; font-size: 0.875rem; color: #334155; }
        input, select, textarea { width: 100%; padding: 10px; border: 1px solid #cbd5e1; border-radius: 4px; box-sizing: border-box; }
        .checkbox-group { display: flex; align-items: center; gap: 8px; margin-top: 16px; }
        .checkbox-group input { width: auto; }
        button { background: #2563eb; color: white; padding: 12px 20px; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; width: 100%; margin-top: 24px; }
        button:hover { background: #1d4ed8; }
      </style>
    </head>
    <body>
      <div class="container">
        <h1 data-testid="page-title">Schedule Medical Equipment Order</h1>
        <form id="orderForm" onsubmit="event.preventDefault(); submitOrder();">
          <label for="patientSelect">Select Patient (HIPAA Verified)</label>
          <select id="patientSelect" data-testid="select-patient" required>
            <option value="">-- Choose Patient --</option>
          </select>

          <label for="equipmentSelect">Select Equipment</label>
          <select id="equipmentSelect" data-testid="select-equipment" required>
            <option value="">-- Choose Equipment --</option>
          </select>

          <label for="deliveryDate">Requested Delivery Date</label>
          <input type="date" id="deliveryDate" data-testid="input-delivery-date" required/>

          <label for="deliveryNotes">Special Clinical Instructions / Access Notes</label>
          <textarea id="deliveryNotes" data-testid="input-delivery-notes" rows="3" placeholder="e.g. 2nd floor, patient requires oxygen setup assistance"></textarea>

          <div class="checkbox-group">
            <input type="checkbox" id="insuranceCovered" data-testid="chk-insurance-coverage" checked/>
            <label for="insuranceCovered" style="margin:0;">Bill Primary Insurance (80% Coverage)</label>
          </div>

          <button type="submit" data-testid="btn-submit-order">Dispatch & Generate Order</button>
        </form>
      </div>
      <script>
        async function init() {
          const [pts, eqs] = await Promise.all([
            fetch('/api/v1/patients').then(r => r.json()),
            fetch('/api/v1/equipment').then(r => r.json())
          ]);
          const pSelect = document.getElementById('patientSelect');
          pts.forEach(p => {
            const opt = document.createElement('option');
            opt.value = p.id;
            opt.textContent = `${p.fullName} (${p.insuranceNumber})`;
            pSelect.appendChild(opt);
          });
          const eSelect = document.getElementById('equipmentSelect');
          eqs.forEach(e => {
            const opt = document.createElement('option');
            opt.value = e.id;
            opt.textContent = `${e.name} - $${e.dailyRate}/day`;
            eSelect.appendChild(opt);
          });
          const tomorrow = new Date();
          tomorrow.setDate(tomorrow.getDate() + 1);
          document.getElementById('deliveryDate').value = tomorrow.toISOString().split('T')[0];
        }
        async function submitOrder() {
          const payload = {
            patientId: document.getElementById('patientSelect').value,
            equipmentIds: [document.getElementById('equipmentSelect').value],
            scheduledDeliveryDate: document.getElementById('deliveryDate').value,
            deliveryNotes: document.getElementById('deliveryNotes').value,
            isInsuranceCovered: document.getElementById('insuranceCovered').checked
          };
          const res = await fetch('/api/v1/orders', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
          });
          const ord = await res.json();
          window.location.href = `/orders/${ord.id}`;
        }
        init();
      </script>
    </body>
    </html>
    """;

    public static string GetOrderDetailsPageHtml(MockOrder order, MockInvoice? invoice) => $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8"/>
      <title>Order Confirmation - Glorium MedTech</title>
      <style>
        body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; background: #f8fafc; padding: 32px; margin: 0; }
        .container { max-width: 700px; margin: 0 auto; background: white; padding: 32px; border-radius: 8px; border: 1px solid #e2e8f0; }
        .status-badge { background: #dcfce7; color: #15803d; padding: 4px 10px; border-radius: 12px; font-weight: 600; font-size: 0.85rem; }
        .grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin: 20px 0; }
        .box { background: #f1f5f9; padding: 16px; border-radius: 6px; }
        .invoice-card { margin-top: 24px; border: 2px dashed #94a3b8; padding: 20px; border-radius: 6px; }
        .btn-home { display: inline-block; margin-top: 24px; color: #2563eb; text-decoration: none; font-weight: 600; }
      </style>
    </head>
    <body>
      <div class="container">
        <div style="display: flex; justify-content: space-between; align-items: center;">
          <h1 style="margin: 0; font-size: 1.5rem;" data-testid="order-header">Order Confirmation</h1>
          <span class="status-badge" data-testid="order-status">{{order.Status}}</span>
        </div>
        <p style="color: #64748b;">Order #: <strong data-testid="order-number">{{order.OrderNumber}}</strong></p>

        <div class="grid">
          <div class="box">
            <h4 style="margin: 0 0 8px 0; color: #475569;">Patient Details</h4>
            <div data-testid="patient-name"><strong>{{order.PatientName}}</strong></div>
            <div style="color: #64748b; font-size: 0.875rem;">Delivery Date: <span data-testid="delivery-date">{{order.ScheduledDeliveryDate}}</span></div>
          </div>
          <div class="box">
            <h4 style="margin: 0 0 8px 0; color: #475569;">Equipment Dispatched</h4>
            {{string.Join("", order.EquipmentItems.Select(eq => $"<div data-testid='ordered-item'>{eq.Name}</div>"))}}
          </div>
        </div>

        {{(invoice != null ? $$"""
        <div class="invoice-card" data-testid="invoice-section">
          <h3 style="margin-top: 0; color: #1e3a8a;">SaaS Billing Invoice: <span data-testid="invoice-number">{{invoice.InvoiceNumber}}</span></h3>
          <p>Monthly Rental Subtotal: <strong data-testid="invoice-subtotal">${{invoice.SubTotal:F2}}</strong></p>
          <p style="color: #15803d;">Insurance Covered: <strong data-testid="insurance-coverage">-${{invoice.InsuranceCoverageAmount:F2}}</strong></p>
          <p style="font-size: 1.2rem; margin: 0; color: #0f172a;">Patient Copay Due: <strong data-testid="patient-copay">${{invoice.PatientCopayAmount:F2}}</strong></p>
        </div>
        """ : "")}}

        <a href="/equipment" class="btn-home" data-testid="btn-back-catalog">← Back to Equipment Catalog</a>
      </div>
    </body>
    </html>
    """;
}
