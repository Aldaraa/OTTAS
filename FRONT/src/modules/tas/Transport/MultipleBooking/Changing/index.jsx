import React from 'react'

function Changing({data}) {
  return (
    <>
      <div className='mt-0 text-xs'>
        <div className='flex justify-end items-center mb-2'>
          <Button type='danger' onClick={handleDeleteAllOnSite} disabled={disabledStatusOnSiteDeleteAllBtn} loading={isLoadingDeleteAllBtn}>
            On Site Delete all
          </Button>
        </div>
        <Table
          data={previewData}
          columns={errorColumns}
          containerClass='shadow-none border overflow-hidden' 
          renderDetail={{enabled: true, component: WarningDetail}}
          pager={previewData?.length > 20}
        />
      </div>
      <div className='flex justify-end gap-5 items-center mt-3'>
        <Button 
          type='primary'
          loading={processLoading}
          onClick={() => handleProcess()}
        >
          Process
        </Button>
        <Button  
          onClick={() => setShowModal(false)}
          disabled={processLoading}
        >
          Cancel
        </Button>
      </div>
    </>
  )
}

export default Changing