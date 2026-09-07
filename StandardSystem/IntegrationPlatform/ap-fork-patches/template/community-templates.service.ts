import { ActivepiecesError, ErrorCode, SeekPage } from '@activepieces/core-utils'
import { ListTemplatesRequestQuery, Template } from '@activepieces/shared'
import { ONFINITY_TEMPLATES, listOnfinityTemplates } from './onfinity-templates'

// Onfinity build: the template gallery serves ONLY curated Onfinity flows (see onfinity-templates.ts),
// not the Activepieces cloud catalog. To restore the cloud source, revert this file.

export const communityTemplates = {
    getOrThrow: async (id: string): Promise<Template> => {
        const template = ONFINITY_TEMPLATES.find((t) => t.id === id)
        if (!template) {
            throw new ActivepiecesError({
                code: ErrorCode.ENTITY_NOT_FOUND,
                params: {
                    entityType: 'template',
                    entityId: id,
                    message: `Template ${id} not found`,
                },
            })
        }
        return template
    },
    getCategories: async (): Promise<string[]> => {
        return ['Onfinity']
    },
    list: async (request: ListTemplatesRequestQuery): Promise<SeekPage<Template>> => {
        const search = (request as { search?: string }).search
        return listOnfinityTemplates(search)
    },
}
