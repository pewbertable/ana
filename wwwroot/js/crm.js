// Initialize SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/crmHub")
    .withAutomaticReconnect()
    .build();

// Start the connection
connection.start().catch(err => console.error(err));

// Handle real-time dashboard updates
connection.on("ReceiveDashboardUpdate", (data) => {
    updateDashboardUI(JSON.parse(data));
});

// Handle real-time client updates
connection.on("ReceiveClientUpdate", (clientId, data) => {
    updateClientUI(clientId, JSON.parse(data));
});

// Handle real-time activity notifications
connection.on("ReceiveClientActivity", (clientId, activity) => {
    showActivityNotification(clientId, activity);
});

// AJAX functions for CRM operations
const crmApi = {
    // Get dashboard data
    getDashboardData: async () => {
        try {
            const response = await fetch('/api/crm/dashboard', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            return await response.json();
        } catch (error) {
            console.error('Error fetching dashboard data:', error);
            throw error;
        }
    },

    // Get client data
    getClientData: async (clientId) => {
        try {
            const response = await fetch(`/api/crm/clients/${clientId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            return await response.json();
        } catch (error) {
            console.error('Error fetching client data:', error);
            throw error;
        }
    },

    // Update client data
    updateClient: async (clientId, data) => {
        try {
            const response = await fetch(`/api/crm/clients/${clientId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            return await response.json();
        } catch (error) {
            console.error('Error updating client:', error);
            throw error;
        }
    }
};

// UI update functions
function updateDashboardUI(data) {
    // Update charts
    if (data.salesChart) {
        updateChart('salesChart', data.salesChart);
    }
    if (data.leadsChart) {
        updateChart('leadsChart', data.leadsChart);
    }
    
    // Update KPI cards
    document.getElementById('totalSales').textContent = data.totalSales;
    document.getElementById('totalLeads').textContent = data.totalLeads;
    document.getElementById('conversionRate').textContent = data.conversionRate + '%';
}

function updateClientUI(clientId, data) {
    const clientCard = document.querySelector(`[data-client-id="${clientId}"]`);
    if (clientCard) {
        clientCard.querySelector('.client-name').textContent = data.name;
        clientCard.querySelector('.client-email').textContent = data.email;
        clientCard.querySelector('.client-status').textContent = data.status;
    }
}

function showActivityNotification(clientId, activity) {
    const notification = document.createElement('div');
    notification.className = 'activity-notification';
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas fa-bell"></i>
            <span>New activity for client ${clientId}: ${activity}</span>
        </div>
    `;
    document.body.appendChild(notification);
    setTimeout(() => notification.remove(), 5000);
}

function updateChart(chartId, data) {
    const chart = Chart.getChart(chartId);
    if (chart) {
        chart.data = data;
        chart.update();
    }
}

// Event listeners for UI interactions
document.addEventListener('DOMContentLoaded', () => {
    // Load initial dashboard data
    crmApi.getDashboardData()
        .then(data => updateDashboardUI(data))
        .catch(error => console.error('Error loading dashboard:', error));

    // Set up client card click handlers
    document.querySelectorAll('.client-card').forEach(card => {
        card.addEventListener('click', async () => {
            const clientId = card.dataset.clientId;
            try {
                const data = await crmApi.getClientData(clientId);
                showClientDetails(data);
            } catch (error) {
                console.error('Error loading client details:', error);
            }
        });
    });
}); 