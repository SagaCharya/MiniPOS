<template>
  <div class="min-h-screen bg-slate-950 p-6 lg:p-10 font-sans text-slate-100">
    <!-- Header -->
    <header class="flex justify-between items-center mb-10">
      <div>
        <h1 class="text-3xl font-black tracking-tight text-white flex items-center gap-3">
          <span class="h-8 w-1.5 bg-orange-600 rounded-full"></span>
          Kitchen Display System
        </h1>
        <p class="text-slate-500 font-medium mt-1">Live order tracking and preparation</p>
      </div>
      <div class="flex items-center gap-6">
        <div class="text-right">
          <div class="text-[10px] uppercase font-black tracking-widest text-slate-500 mb-1">System Status</div>
          <div class="flex items-center gap-2 text-green-500 font-bold text-sm">
            <span class="h-2 w-2 bg-green-500 rounded-full animate-pulse"></span>
            Connected & Live
          </div>
        </div>
        <button @click="fetchOrders" class="p-3 bg-slate-900 hover:bg-slate-800 rounded-2xl transition-all border border-slate-800">
          <svg xmlns="http://www.w3.org/2000/svg" :class="['h-5 w-5 text-slate-400', isLoading ? 'animate-spin' : '']" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
        </button>
      </div>
    </header>

    <!-- Orders Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <div 
        v-for="order in activeOrders" 
        :key="order.id"
        class="bg-slate-900 rounded-[32px] border border-slate-800/50 overflow-hidden shadow-2xl flex flex-col"
      >
        <!-- Card Header -->
        <div :class="['p-6 flex justify-between items-start', getStatusColor(order.status)]">
          <div>
            <div class="text-[10px] font-black uppercase tracking-[0.2em] opacity-60 mb-1">Table #{{ order.table?.tableNumber || '??' }}</div>
            <h2 class="text-2xl font-black">Order #{{ order.id }}</h2>
          </div>
          <div class="bg-black/20 backdrop-blur px-3 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-wider">
            {{ formatTime(order.orderDate) }}
          </div>
        </div>

        <!-- Items List -->
        <div class="p-6 flex-1 space-y-4 max-h-[400px] overflow-y-auto custom-scrollbar">
          <div v-for="item in order.orderItems" :key="item.id" class="flex gap-4 items-start group">
            <div class="h-10 w-10 bg-slate-800 rounded-xl flex items-center justify-center font-black text-slate-400 border border-slate-700/50 flex-shrink-0">
              {{ item.quantity }}x
            </div>
            <div class="flex-1 min-w-0">
              <div class="font-bold text-slate-200 leading-tight">{{ item.product?.name }}</div>
              <div v-if="item.specialInstructions" class="text-xs text-orange-500/80 font-medium mt-1 italic">
                "{{ item.specialInstructions }}"
              </div>
            </div>
          </div>
        </div>

        <!-- Status Actions -->
        <div class="p-6 bg-slate-900/50 border-t border-slate-800/50">
          <button 
            v-if="order.status === 0 || order.status === 1"
            @click="updateStatus(order, 2)"
            class="w-full bg-orange-600 hover:bg-orange-500 text-white py-4 rounded-2xl font-black transition-all shadow-xl shadow-orange-600/10 active:scale-95 flex items-center justify-center gap-3 uppercase tracking-wider text-sm"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
            Start Preparing
          </button>

          <button 
            v-else-if="order.status === 2"
            @click="updateStatus(order, 3)"
            class="w-full bg-green-600 hover:bg-green-500 text-white py-4 rounded-2xl font-black transition-all shadow-xl shadow-green-600/10 active:scale-95 flex items-center justify-center gap-3 uppercase tracking-wider text-sm"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
            </svg>
            Mark as Ready
          </button>

          <div v-else class="text-center py-4 bg-slate-800/50 rounded-2xl border border-slate-700/50 text-[10px] font-black uppercase tracking-widest text-slate-500">
            Waiting for Waiter
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="activeOrders.length === 0 && !isLoading" class="col-span-full py-40 flex flex-col items-center justify-center text-slate-700">
        <div class="h-24 w-24 bg-slate-900 rounded-[32px] flex items-center justify-center mb-6 border border-slate-800 shadow-2xl">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10 text-slate-800" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
          </svg>
        </div>
        <p class="font-black uppercase tracking-widest text-sm">Kitchen is Clear</p>
        <p class="text-slate-600 mt-2 font-medium">New orders will appear here automatically</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'

const orders = ref([])
const isLoading = ref(false)
let pollInterval = null

const activeOrders = computed(() => {
  // Show Pending (0), Confirmed (1), Preparing (2), Ready (3)
  // Served (4) and Paid (5) hide from KDS
  return orders.value.filter(o => o.status >= 0 && o.status <= 3)
    .sort((a, b) => b.status - a.status || new Date(a.orderDate) - new Date(b.orderDate))
})

const fetchOrders = async () => {
  isLoading.value = true
  try {
    const res = await fetch(`/api/orders/pending?t=${Date.now()}`)
    if (res.ok) orders.value = await res.json()
  } catch (error) {
    console.error('KDS Fetch Error:', error)
  } finally {
    isLoading.value = false
  }
}

const updateStatus = async (order, status) => {
  try {
    const res = await fetch(`/api/orders/${order.id}/status`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ status })
    })
    
    if (res.ok) {
      // Optimistic update
      order.status = status
      await fetchOrders() // Refresh to be safe
    }
  } catch (e) {
    console.error('Status Update Error:', e)
  }
}

const getStatusColor = (status) => {
  switch (status) {
    case 2: return 'bg-orange-600/20 text-orange-500' // Preparing
    case 3: return 'bg-green-600/20 text-green-500'   // Ready
    default: return 'bg-slate-800 text-slate-400'    // Pending/Confirmed
  }
}

const formatTime = (dateStr) => {
  const date = new Date(dateStr)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

onMounted(() => {
  fetchOrders()
  pollInterval = setInterval(fetchOrders, 5000)
})

onUnmounted(() => {
  if (pollInterval) clearInterval(pollInterval)
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #334155;
  border-radius: 10px;
}
</style>
